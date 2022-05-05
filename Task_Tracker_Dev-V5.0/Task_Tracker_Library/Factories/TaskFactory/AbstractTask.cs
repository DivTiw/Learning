using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;

namespace Task_Tracker_Library.Factories.TaskFactory
{
    public abstract class AbstractTask
    {
        ///ToDo: Common code list to be handled here:
        /// 1. Adding trail
        /// 2. Sending Email
        /// 3. Adding Members
        /// 4. Checking for diabled statuses
        /// 5. Checking for disabled fields on the screen
        protected TaskDM _taskDM;
        protected UnitOfWork _uow = null;
        protected List<string> _email_activity;
        protected DateTime _trailTime;
        protected int _trail_groupID;
        protected Enum_Master.TaskType _task_type;
        protected Email_Enums.Email_Type _email_type;
        protected bool TaskStatusHasChanged { get; private set; } = false;
        protected string TaskStatus { get; set; }

        protected Enum_Master.TaskOperationState _taskOperation;

        public virtual void Attach(TaskDM taskDM, UnitOfWork uow = null)
        {
            if (taskDM == null)
                throw new ArgumentNullException("Task object can not be null");

            _taskDM = taskDM;
            _uow = uow;

            if (_taskDM.created_by == 0 && _taskDM.logged_in_user == 0)
                throw new NullReferenceException("Logged in user can not be null.");

            if (_taskDM.task_status_syscode > 0)
                TaskStatus = ((Enum_Master.StatusEnum)_taskDM.task_status_syscode).GetDescription();

            if (_uow != null)
            {
                _trail_groupID = _uow.TasksRepo.getMaxTrailGroupID(_taskDM.task_syscode) + 1;
            }
        }

        public virtual void Attach(TaskDM taskDM, Enum_Master.TaskOperationState task_Op, Email_Enums.Email_Type email_Typ, UnitOfWork uow = null)
        {
            _taskOperation = task_Op;
            _email_type = email_Typ;
            Attach(taskDM, uow);
        }

        public virtual void InitialiseTrail()
        {
            _taskDM.lstTrail = new List<TaskTrail>();
            _trailTime = DateTime.Now;
            _email_activity = new List<string>();
            if (!string.IsNullOrEmpty(_taskDM.task_comment))
            {
                CreateTrail(Enum_Master.ActivityEnum.Added_Comments);
            }
            if (_taskDM.arrCCUsersSyscodes != null)
            {
                CreateTrail(Enum_Master.ActivityEnum.Informed_To);
            }
        }

        public TaskDM GetTaskObject()
        {
            return _taskDM;
        }

        private void InsertUpdateProgress(UnitOfWork _uow, decimal _curProgress, string _progressType, int _typeSyscode, int _userId)
        {
            if (_uow == null || string.IsNullOrEmpty(_progressType) || _typeSyscode == 0) return;

            ProgressMaster pm = _uow.ProgressRepo.GetSingle(x => x.type_detail.Equals(_progressType) && x.type_syscode == _typeSyscode);
            if (pm != null && pm.progress_syscode > 0)
            {
                pm.progress = pm.progress + _curProgress;
                pm.modified_by = _userId;
                pm.modified_on = DateTime.Now;

                _uow.ProgressRepo.Update(pm);
            }
            else
            {
                pm = new ProgressMaster();
                pm.type_detail = _progressType;
                pm.type_syscode = _typeSyscode;
                pm.progress = _curProgress;
                pm.created_by = _userId;

                _uow.ProgressRepo.Add(pm);
            }
        }

        private void UpdateProgress()
        {
            //Order of statements in this funtion matters, any change in order will affect the accuracy of the progress calculated.
            //using (var uow = new UnitOfWork())
            //{
            //Return if the current task is already 100% complete in the database.
            if (_uow.ProgressRepo.Any(x => x.type_syscode == _taskDM.task_syscode && x.progress == 100))
                return;

            JMTask curTask;

            int? iTaskSyscode = _taskDM.task_syscode;
            decimal curTaskProgress = 100;

            do
            {
                ///ToDo: Severity: Very Low :: Optimize this statement to fetch the Task list from database for each iteration.
                /// The list of related tasks can be braught in just single call and then the iterations can be performed on that list.
                /// Or this performance overhead can be considered for tackling the concurrent write from the Weighatage updation view, so that it reads fresh update of the weightage.
                /// In the long run above statement will be true considering there is no way to synchronize two views working on the same table and column but could be requested from the different clients altogether.
                curTask = _uow.TasksRepo.GetSingle(x => x.task_syscode == iTaskSyscode);

                //Progress updation for Tasks
                InsertUpdateProgress(_uow, curTaskProgress, Enum_Master.ProgressTypeEnum.Task.ToString(), curTask.task_syscode, _taskDM.created_by);

                if (curTask.weightage <= 0)
                    break;
                //throw new InvalidOperationException($"Can not complete the task; Weightage is less than or equal to zero for task with reference: {curTask.task_reference} in the chain of parent tasks.");

                //Formula for calculating the progress for Parent of current task.
                curTaskProgress = (curTaskProgress * curTask.weightage) / 100;

                iTaskSyscode = curTask.parent_task_syscode;

                if (iTaskSyscode == null || iTaskSyscode == 0) break;

            } while (curTask != null);

            // Progress updation for Module
            InsertUpdateProgress(_uow, curTaskProgress, Enum_Master.ProgressTypeEnum.Module.ToString(), curTask.module_syscode ?? 0, _taskDM.created_by);

            _uow.commitTT();
            //}
        }
        public void ChangeStatus()
        {
            JMTask objTask = _uow.TasksRepo.Get(_taskDM.task_syscode);

            if (objTask != null)
            {
                if (objTask.task_status_syscode != _taskDM.task_status_syscode)
                {
                    ///ToDo: Find better place for writting below code.
                    if (_taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Complete)
                    {
                        if (_uow.TaskUserRecordRepo.Any(x => x.task_syscode == _taskDM.task_syscode && x.start_time != null && x.stop_time == null))
                        {
                            throw new InvalidOperationException("Can not complete the task; It is open with one or multiple user.");
                        }
                        if (_task_type == Enum_Master.TaskType.Workflow)
                            UpdateProgress();
                    }
                    objTask.modified_by = _taskDM.created_by;
                    objTask.modified_on = DateTime.Now;
                    objTask.task_status_syscode = _taskDM.task_status_syscode;
                    _uow.TasksRepo.Update(objTask);
                    _uow.commitTT();
                    TaskStatusHasChanged = true;
                    CreateTrail(Enum_Master.ActivityEnum.Changed_Status, _taskDM.task_status_syscode);
                }
                else if (_taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Discard)
                {
                    throw new InvalidOperationException("This task has been marked Discard and no further operation is possible.");
                }
                else if (_taskDM.task_status_syscode == (int)Enum_Master.StatusEnum.Complete)
                {
                    throw new InvalidOperationException("This task has been marked Complete and no further operation is possible.");
                }
            }
        }

        private string GetMembersName(List<TaskUserMapping> lstMembers)
        {
            return GetMembersName(lstMembers.Select(x => x.employee_syscode).ToList());
        }

        private string GetMembersName(List<int> lstUserSyscodes)
        {
            string memberNames = string.Empty;

            var memNames = _uow.EmployeeRepo.GetNamesBySyscode(lstUserSyscodes);
            memberNames = string.Join(", ", memNames.ToArray());
            return memberNames;
        }

        public virtual void CreateTrail(Enum_Master.ActivityEnum activity, object arrTrailInfo = null)
        {
            TaskTrail trail = new TaskTrail();

            string activityDesc = activity.GetDescription();
            _email_activity.Add(activityDesc);

            trail.trail_group_id = _trail_groupID;
            trail.task_syscode = _taskDM.task_syscode;
            trail.activity_syscode = (int)activity;
            trail.trail_start_datetime = _trailTime;
            trail.trail_description = _taskDM.logged_in_user_name + " " + activityDesc + " on " + _trailTime.ToString("dd/MM/yyyy HH:mm:ss");
            trail.created_on = _trailTime;

            switch (activity)
            {
                case Enum_Master.ActivityEnum.Created:
                    break;
                case Enum_Master.ActivityEnum.Acknowledged:
                    break;
                case Enum_Master.ActivityEnum.Started:
                    break;
                case Enum_Master.ActivityEnum.Changed_Weightage:
                    break;
                case Enum_Master.ActivityEnum.Created_For:
                    break;
                case Enum_Master.ActivityEnum.Forwarded:
                    break;
                case Enum_Master.ActivityEnum.Completed:
                    break;
                case Enum_Master.ActivityEnum.Closed:
                    break;
                case Enum_Master.ActivityEnum.Changed_Status:
                    trail.trail_comments = "To " + TaskStatus;// + obj.options[obj.selectedIndex].text
                    break;
                case Enum_Master.ActivityEnum.Added_File:
                    trail.trail_comments = "File(s) : " + string.Join(", ", arrTrailInfo as string[]);// + fileName
                    break;
                case Enum_Master.ActivityEnum.Added_Comments:
                    trail.trail_comments = _taskDM.task_comment;
                    break;
                case Enum_Master.ActivityEnum.Created_Subtask:
                    break;
                case Enum_Master.ActivityEnum.Added_Member:
                case Enum_Master.ActivityEnum.Removed_Member:
                    trail.trail_comments = GetMembersName(arrTrailInfo as List<TaskUserMapping>);
                    break;
                case Enum_Master.ActivityEnum.Informed_To:
                    if (_taskDM.arrCCUsersSyscodes != null)
                    {
                        trail.inform_to = string.Join(",", _taskDM.arrCCUsersSyscodes);
                        trail.trail_comments = GetMembersName(_taskDM.arrCCUsersSyscodes.ToList());
                    }
                    //trail.is_deleted = true; 
                    break;
                default:
                    break;
            }

            _taskDM.lstTrail.Add(trail);
        }

        public virtual void AddTrails()
        {
            if (_taskDM.lstTrail != null || _taskDM.lstTrail.Count > 0)
            {
                _uow.TaskTrailRepo.AddRange(_taskDM.lstTrail);
                _uow.commitTT();
            }
        }

        public virtual OperationDetailsDTO UpdateMembers()
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            if (_taskDM.arrUserSyscodes != null && _taskDM.arrUserSyscodes.Length > 0)//_task.lstTrail.Any(t => t.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Member)
            {
                //uow.commitTT();

                List<TaskUserMapping> taskUsers = new List<TaskUserMapping>();

                foreach (var arrItem in _taskDM.arrUserSyscodes)
                {
                    TaskUserMapping user = new TaskUserMapping();
                    user.employee_syscode = arrItem;
                    user.user_role_syscode = (int)Enum_Master.UserRoleEnum.Created_For;
                    ///ToDo: Very Imp: High Priority: Find the way to add trail syscode here when trails are still getting formed and not yet added to the DB.
                    //user.trail_syscode = _task.lstTrail.FirstOrDefault(x => x.activity_syscode == (int)Enum_Master.ActivityEnum.Added_Member).trail_syscode;
                    user.task_syscode = _taskDM.task_syscode;
                    user.created_by = _taskDM.created_by;
                    taskUsers.Add(user);
                }

                List<TaskUserMapping> TaskUserMappingsinDB = _uow.TaskUserMappingRepo
                                                                .GetList(x => x.task_syscode.Equals(_taskDM.task_syscode)
                                                                            && !x.is_deleted
                                                                            && x.is_active)
                                                                ?.ToList();
                List<TaskUserMapping> lstNewTskUsr = null, lstDelTskUsr = null;
                if (TaskUserMappingsinDB != null && TaskUserMappingsinDB.Count > 0)
                {

                    //List of users removed from mapping from the UI and needs to be deleted and made is active false.
                    lstDelTskUsr = TaskUserMappingsinDB.Where(x => !taskUsers.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                    //Get users which are not already in DB
                    lstNewTskUsr = taskUsers.Where(x => !TaskUserMappingsinDB.Any(y => y.employee_syscode.Equals(x.employee_syscode))).ToList();

                    if ((lstDelTskUsr == null || lstDelTskUsr.Count == 0) && (lstNewTskUsr == null || lstNewTskUsr.Count == 0))
                    {
                        od.opStatus = true;
                        return od;
                    }
                    //Process for deleting the mappings which are not present in the UI. Changing the property value of list to make it deleted.
                    lstDelTskUsr.ForEach(x => { x.is_deleted = true; x.is_active = false; x.modified_by = _taskDM.created_by; x.modified_on = DateTime.Now; });

                    _uow.TaskUserMappingRepo.UpdateRange(lstDelTskUsr);
                }
                else
                {
                    lstNewTskUsr = taskUsers;
                }
                _uow.TaskUserMappingRepo.AddRange(lstNewTskUsr);
                _uow.commitTT();
                if (lstDelTskUsr != null && lstDelTskUsr.Count > 0)
                    CreateTrail(Enum_Master.ActivityEnum.Removed_Member, lstDelTskUsr);
                if (lstNewTskUsr != null && lstNewTskUsr.Count > 0)
                    CreateTrail(Enum_Master.ActivityEnum.Added_Member, lstNewTskUsr);

                od.opStatus = true;
                od.opMsg = "Members successfully updated.";
            }
            else
            {
                od.opStatus = true;
                od.opMsg = "Members array is empty and nothing to update.";
            }
            return od;
            //throw new NotImplementedException();
        }

        public virtual OperationDetailsDTO SendEmail()
        {
            OperationDetailsDTO od = new OperationDetailsDTO();

            EmailTemplate temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Task_Updated") && x.is_active).FirstOrDefault();
            if (temp == null)
            {
                throw new Exception("Email Template Not found.");
            }
            List<int> to_syscodes = new List<int>();

            to_syscodes.Add(_taskDM.created_by);
            if (_taskDM.arrUserSyscodes != null)
                to_syscodes.AddRange(_taskDM.arrUserSyscodes);
            to_syscodes.Add(_taskDM.task_owner);
            to_syscodes.Add(_taskDM.task_on_behalf);
            to_syscodes.RemoveAll(item => item == 0);

            int template_syscode = temp.template_syscode;
            string email_subject = temp.template_subject;
            string email_from_display = temp.from_email_display;
            string email_from_id = temp.from_email_id;
            string email_body = temp.template_body;
            string strMembers = string.Empty;
            if (_taskDM.arrUserSyscodes != null)
                strMembers = _uow.CommonRepo.getEmployeeNames(_taskDM.arrUserSyscodes);
            if (_taskDM.arrCCUsersSyscodes != null)
            {
                to_syscodes.AddRange(_taskDM.arrCCUsersSyscodes);
                strMembers = strMembers + ", " + _uow.CommonRepo.getEmployeeNames(_taskDM.arrCCUsersSyscodes);
            }
            string email_link = temp.link_url + "?returnValue=Tasks/Task/ViewTask/" + ComLibCommon.Base64Encode(_taskDM.task_syscode + "");
            string email_rows = "<tr><td><a href=\"" + email_link + "\">" + _taskDM.task_reference + "</a></td><td>" + _taskDM.task_subject +
                                "</td><td>" + strMembers + "</td></tr>";

            if (TaskStatusHasChanged)
            {
                email_body = email_body.Replace("updated", "changed to <b>" + TaskStatus + "</b> status");
            }
            email_body = email_body.Replace("#emp_name#", "<b>" + _taskDM.logged_in_user_name + "</b>");
            email_body = email_body.Replace("#activity#", string.Join(", ", _email_activity.ToArray()));
            email_body = email_body.Replace("#comments#", _taskDM.task_comment);//strComments
            email_body = email_body.Replace("#rows#", email_rows);

            bool emailSent = _uow.EmailRepo.SendEmail(_taskDM.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes);
            if (!emailSent)
            {
                od.opStatus = false;
                _taskDM.opMsg = "eMail could not be sent.";
            }
            else
            {
                od.opStatus = true;
                od.opMsg = "Email successfully sent.";
            }
            return od;
            //throw new NotImplementedException();
        }

        /// TODO: Handle this To and CC syscodes better.
        public virtual OperationDetailsDTO BuildEmail3(JMTask _task = null, int[] _arrMembers = null, int[] _arrCCUsers = null)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();

            //string strTitle = string.Empty;
            EmailTemplate temp = null;
            List<int> to_syscodes = new List<int>();
            List<int> cc_syscodes = new List<int>();
            string Status_TaskToBeMailed = string.Empty;
            int[] _arrWriteUsers = null;

            if (_task == null)
            {
                _task = _taskDM;
            }

            Status_TaskToBeMailed = ((Enum_Master.StatusEnum)_task.task_status_syscode).GetDescription();

            #region Fetch Users
            if (_arrMembers == null)
            {
                _arrMembers = _taskDM.arrUserSyscodes;
            }
            if (_arrCCUsers == null)
            {
                _arrCCUsers = _taskDM.arrCCUsersSyscodes;
            }

            _arrWriteUsers = (from users in _uow.ProjModUserMappingRepo
                                             .GetList(x => ((x.project_syscode == _taskDM.project.project_syscode
                                                            && x.module_syscode == null
                                                            && x.role_syscode == (int)Enum_Master.UserRoleEnum.Project_User
                                                           ) ||
                                                           (x.project_syscode == _taskDM.project.project_syscode
                                                            && x.module_syscode == _task.module_syscode
                                                            && x.role_syscode == (int)Enum_Master.UserRoleEnum.Module_User
                                                           ))
                                                           && x.access_write && x.is_active && !x.is_deleted
                                                      )
                              select users.employee_syscode)?.ToArray();
            #endregion

            //Mail for Task Created
            #region Fetch Template and Assign To&CC Users
            if (_taskOperation == Enum_Master.TaskOperationState.Creation && _task_type == Enum_Master.TaskType.Workflow)
            {
                if (_task.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate)
                {
                    temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Task_Initiated") && x.is_active).FirstOrDefault();
                }
                else
                {
                    temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("WF_Task_Created") && x.is_active).FirstOrDefault();
                }
                //All the tasks are either created in ToDo | Open | Initiate state.
                if (_task.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate)
                {
                    //All the "Read and Write" access members in the module
                    if (_arrMembers != null)
                        to_syscodes.AddRange(_arrMembers);
                    cc_syscodes.Add(_task.created_by);
                    cc_syscodes.Add(_task.task_owner);
                    cc_syscodes.Add(_task.task_on_behalf);
                }
                else
                {
                    //All the other users than creator should be notified only in case of the parent task and step task.
                    if (_task.parent_task_syscode == null || _task.parent_task_syscode == 0)
                    {
                        to_syscodes.AddRange(_arrMembers);
                        to_syscodes.Add(_task.task_owner);
                        to_syscodes.Add(_task.task_on_behalf);
                    }
                    cc_syscodes.Add(_task.created_by);
                }
            }
            else if (_taskOperation == Enum_Master.TaskOperationState.Creation && _task_type == Enum_Master.TaskType.StandAlone)
            {
                temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("ST_Task_Created") && x.is_active).FirstOrDefault();

                if (_arrMembers != null)
                    to_syscodes.AddRange(_arrMembers);
                cc_syscodes.Add(_task.created_by);
                cc_syscodes.Add(_task.task_owner);
                cc_syscodes.Add(_task.task_on_behalf);
            }
            //Mail for task Updated
            else
            {
                //Currently there is no requirement for sending mails for the open status updates.
                if (_task.task_status_syscode == (int)Enum_Master.StatusEnum.Open)
                {
                    od.opMsg = "Currently sending mail for open status is disabled.";
                    od.opStatus = true;
                    return od;
                }

                //Fetching the creater of the task.
                int taskCreaterSyscode = _uow.TasksRepo.GetSingle(x => x.task_syscode == _task.task_syscode)?.created_by ?? 0;

                //Below code is for adding TO and CC users.
                if (_task.task_status_syscode == (int)Enum_Master.StatusEnum.Initiate)
                {
                    if (_arrMembers != null)
                        to_syscodes.AddRange(_arrMembers);
                    if (_arrCCUsers != null)
                        cc_syscodes.AddRange(_arrCCUsers);
                    cc_syscodes.Add(taskCreaterSyscode);
                    cc_syscodes.Add(_task.task_owner);
                    cc_syscodes.Add(_task.task_on_behalf);
                }
                else
                {
                    if (_arrCCUsers != null) // && if Inform To users are selected  put them in email To list and others in CC
                    {
                        to_syscodes.AddRange(_arrCCUsers);       //InformTo user in To               
                        cc_syscodes.Add(taskCreaterSyscode);
                        if (_arrMembers != null)                //put task members in CC
                            cc_syscodes.AddRange(_arrMembers);

                    }
                    else //Put task creator, task members in email To list and others in CC
                    {
                        to_syscodes.Add(taskCreaterSyscode);
                        if (_arrMembers != null)
                            to_syscodes.AddRange(_arrMembers); //task member in To
                        //if (_arrCCUsers != null) //As soon as Inform-To is having data then it should be in To address.
                        //    cc_syscodes.AddRange(_arrCCUsers); 
                    }
                    cc_syscodes.Add(_taskDM.task_owner);
                    cc_syscodes.Add(_taskDM.task_on_behalf);

                    if (_arrWriteUsers != null)
                        cc_syscodes.AddRange(_arrWriteUsers); //Project, Module Write Users


                }
                to_syscodes.RemoveAll(item => item == 0);
                cc_syscodes.RemoveAll(item => item == 0);

                if (_arrCCUsers != null)
                    temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Other_Updates") && x.is_active).FirstOrDefault();
                else if (TaskStatusHasChanged)
                    temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals(Email_Dictionary.EmailTemplateName[(Enum_Master.StatusEnum)_task.task_status_syscode]) && x.is_active).FirstOrDefault();
                else
                    temp = _uow.EmailTemplateRepo.GetList(x => x.template_name.Equals("Other_Updates") && x.is_active).FirstOrDefault();
            }

            if (temp == null)
            {
                throw new Exception("Email Template Not found. Make sure all the relevant templates are present in the database.");
            }
            #endregion

            #region TemplateBodyReplace
            int template_syscode = temp.template_syscode;
            string email_subject = temp.template_subject;
            string email_from_display = temp.from_email_display;
            string email_from_id = temp.from_email_id;
            string email_body = temp.template_body;
            string strMembers = string.Empty;

            if (_arrMembers != null)
                strMembers = _uow.CommonRepo.getEmployeeNames(_arrMembers);
            if (_arrCCUsers != null)
                strMembers = strMembers + ", " + _uow.CommonRepo.getEmployeeNames(_arrCCUsers);

            string email_link = temp.link_url + ComLibCommon.Base64Encode(_task.task_syscode + "");

            email_subject = email_subject.Replace("#wf_level#", _taskDM.level_name);

            //email_body = email_body.Replace("#title#", strTitle);
            email_body = email_body.Replace("#emp_name#", _taskDM.logged_in_user_name);
            email_body = email_body.Replace("#project#", _taskDM.project?.project_name);
            email_body = email_body.Replace("#category#", _taskDM.category?.category_name);
            email_body = email_body.Replace("#module#", _taskDM.module?.module_name);
            email_body = email_body.Replace("#wf#", _taskDM.workflow_name);
            email_body = email_body.Replace("#subject#", _task.task_subject);
            email_body = email_body.Replace("#status#", Status_TaskToBeMailed);
            email_body = email_body.Replace("#desc#", _task.task_details);
            email_body = email_body.Replace("#comment#", _taskDM.task_comment);
            email_body = email_body.Replace("#wf_level#", _taskDM.level_name);
            email_body = email_body.Replace("#url#", email_link);

            #endregion
            bool emailSent = _uow.EmailRepo.SendEmail(_taskDM.created_by, template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes, cc_syscodes);
            if (!emailSent)
            {
                od.opStatus = false;
                _taskDM.opMsg = "eMail could not be sent.";
            }
            else
            {
                od.opStatus = true;
                od.opMsg = "Email successfully sent.";
            }
            return od;

        }

        public virtual OperationDetailsDTO BuildEmail(JMTask _task = null, int[] _arrMembers = null, int[] _arrCCUsers = null)
        {
            OperationDetailsDTO od = new OperationDetailsDTO();

            EmailTemplateDM Email_Tmplt = null;
            List<int> to_syscodes = new List<int>();
            List<int> cc_syscodes = new List<int>();
            int? task_status_syscode = null;
            string Status_TaskToBeMailed = string.Empty;
            string strMembers = string.Empty;
            string parentTaskSubject = string.Empty;
            //int[] _arrWriteUsers = null;

            if (_task == null)
            {
                _task = _taskDM;
            }
            task_status_syscode = _task.task_status_syscode;
            parentTaskSubject = _uow.TasksRepo.GetSingle(x => x.task_syscode == _task.parent_task_syscode)?.task_subject;
            parentTaskSubject = string.IsNullOrEmpty(parentTaskSubject) ? string.Empty : $"({parentTaskSubject})";

            if (_taskDM.task_status_syscode > 0)
                Status_TaskToBeMailed = ((Enum_Master.StatusEnum)task_status_syscode).GetDescription();

            if (_arrMembers == null)
            {
                _arrMembers = _taskDM.arrUserSyscodes;
            }
            if (_arrCCUsers == null)
            {
                _arrCCUsers = _taskDM.arrCCUsersSyscodes;
            }

            if (_taskOperation == Enum_Master.TaskOperationState.Creation)
            {
                //------
                 if (_task_type == Enum_Master.TaskType.Workflow && (_task.parent_task_syscode == null || _task.parent_task_syscode == 0))
                    Email_Tmplt = _uow.EmailRepo.GetEmailTemplate(Email_Enums.Email_Type.TCWFP, _task_type, null);
                else
                    Email_Tmplt = _uow.EmailRepo.GetEmailTemplate(_email_type, _task_type, task_status_syscode);
            }
            else
            {
                //Currently there is no requirement for sending mails for the open status updates.
                if (_task.task_status_syscode == (int)Enum_Master.StatusEnum.Open)
                {
                    od.opMsg = "Currently sending mail for open status is disabled.";
                    od.opStatus = true;
                    return od;
                }
                if (_arrCCUsers != null)
                {
                    Email_Tmplt = _uow.EmailRepo.GetEmailTemplate(Email_Enums.Email_Type.TINFT, null, null);
                    //strMembers =  _uow.CommonRepo.getEmployeeNames(_arrCCUsers);
                }
                else if (TaskStatusHasChanged)
                    Email_Tmplt = _uow.EmailRepo.GetEmailTemplate(_email_type, null, task_status_syscode);
                else
                    Email_Tmplt = _uow.EmailRepo.GetEmailTemplate(_email_type, null, null);
            }

            if (Email_Tmplt == null)
            {
                od.opMsg = "Email Template or Email Definition not found or is not active.";
                Log.LogDebug(od.opMsg, "", _taskDM.logged_in_user_name, "BuildEmail", "AbstractTask");
                od.opStatus = true;
                return od;
                //throw new Exception("Email Template Not found. Make sure all the relevant templates are present in the database.");
            }

            to_syscodes = returnRecipientsSyscode(Email_Tmplt.str_To_CatCodes, _task, _arrCCUsers);
            cc_syscodes = returnRecipientsSyscode(Email_Tmplt.str_Cc_CatCodes, _task, _arrCCUsers);

            if (to_syscodes != null && to_syscodes.Count > 0)
            {
                strMembers = _uow.CommonRepo.getEmployeeNames(to_syscodes.ToArray());
            }

            #region TemplateBodyReplace
            int template_syscode = Email_Tmplt.template_syscode;
            string email_subject = Email_Tmplt.template_subject;
            string email_from_display = Email_Tmplt.from_email_display;
            string email_from_id = Email_Tmplt.from_email_id;
            string email_body = Email_Tmplt.template_body;    
            string email_link = Email_Tmplt.link_url + ComLibCommon.Base64Encode(_task.task_syscode + "");

            email_subject = email_subject.Replace("#project_name#", _taskDM.project?.project_name);
            email_subject = email_subject.Replace("#module_name#", _taskDM.module?.module_name);
            email_subject = email_subject.Replace("#task_ref#", _taskDM.task_reference);

            if(_task_type == Enum_Master.TaskType.StandAlone)
                email_subject = email_subject.Replace("[ | #wf_level#]", " | Standalone");
            else if(_task_type == Enum_Master.TaskType.Workflow && (_task.parent_task_syscode == null || _task.parent_task_syscode == 0))
                email_subject = email_subject.Replace("[ | #wf_level#]", " | Root");
            else
                email_subject = email_subject.Replace("[ | #wf_level#]", " | " + _taskDM.level_name);

            //if (string.IsNullOrEmpty(_taskDM.level_name))
            //    email_subject = email_subject.Replace("[ | #wf_level#]", "Standalone");  
            //else
            //    email_subject = email_subject.Replace("[ | #wf_level#]", " | " +_taskDM.level_name);

            email_body = email_body.Replace("#to_emp#", strMembers);
            email_body = email_body.Replace("#emp_name#", _taskDM.logged_in_user_name);
            email_body = email_body.Replace("#project#", _taskDM.project?.project_name);
            email_body = email_body.Replace("#category#", _taskDM.category?.category_name);
            email_body = email_body.Replace("#module#", _taskDM.module?.module_name);
            email_body = email_body.Replace("#wf#", _taskDM.workflow_name);
            email_body = email_body.Replace("#subject#", $"{_task.task_subject} {parentTaskSubject}");
            email_body = email_body.Replace("#status#", Status_TaskToBeMailed);
            email_body = email_body.Replace("#desc#", _task.task_details);
            email_body = email_body.Replace("#comment#", _taskDM.task_comment);
            email_body = email_body.Replace("#wf_level#", _taskDM.level_name);
            email_body = email_body.Replace("#url#", email_link);

            email_from_display = email_from_display.Replace("#emp_name#", _taskDM.logged_in_user_name);
            #endregion

            bool emailSent = _uow.EmailRepo.SendEmail(_taskDM.created_by, Email_Tmplt.template_syscode, email_subject, email_from_display, email_from_id, email_body, to_syscodes, cc_syscodes);
            if (!emailSent)
            {
                od.opStatus = false;
                _taskDM.opMsg = "eMail could not be sent.";
            }
            else
            {
                od.opStatus = true;
                od.opMsg = "Email successfully sent.";
            }
            return od;
        }

        public virtual OperationDetailsDTO SendSMS()
        {
            throw new NotImplementedException();
        }

        public virtual OperationDetailsDTO Process()
        {
            OperationDetailsDTO od = new OperationDetailsDTO();
            ChangeStatus();
            od.opStatus = true;
            return od;
        }

        public virtual OperationDetailsDTO Complete()
        {
            throw new NotImplementedException();
        }


        private List<int> returnRecipientsSyscode(string recepCategories, JMTask _task = null, int[] _arrCCUsers = null)
        {
            List<int> lstRecipient = new List<int>();
            if (string.IsNullOrEmpty(recepCategories))
                return lstRecipient;

            string[] arrRecepCat = null;
            arrRecepCat = recepCategories.Split(',');
            foreach (string cat in arrRecepCat)
            {
                Email_Enums.Email_Recipients enmRecp = cat.ConvertToEnum<Email_Enums.Email_Recipients>();
                switch (enmRecp)
                {
                    case Email_Enums.Email_Recipients.TMEM:
                        int[] arrMembers = _uow.TaskUserMappingRepo.GetList(x => x.task_syscode == _task.task_syscode && x.is_active && !x.is_deleted).Select(x => x.employee_syscode)?.ToArray();
                        if (arrMembers != null)
                            lstRecipient.AddRange(arrMembers);
                        break;
                    case Email_Enums.Email_Recipients.TWNR:
                        lstRecipient.Add(_task.task_owner);
                        break;
                    case Email_Enums.Email_Recipients.TONBF:
                        lstRecipient.Add(_task.task_on_behalf);
                        break;
                    case Email_Enums.Email_Recipients.TCRTR:
                        //Fetching the creater of the task.
                        int taskCreaterSyscode = _uow.TasksRepo.GetSingle(x => x.task_syscode == _task.task_syscode)?.created_by ?? 0;
                        lstRecipient.Add(taskCreaterSyscode);
                        break;
                    case Email_Enums.Email_Recipients.TINFT:
                        if (_arrCCUsers != null)
                            lstRecipient.AddRange(_arrCCUsers);
                        break;
                    case Email_Enums.Email_Recipients.PMWrite:
                        int[] arrPMWUsers = _uow.ProjModUserMappingRepo.getProjModWriteUsers(_task.module_syscode ?? 0, _taskDM.project?.project_syscode?? 0);
                        if (arrPMWUsers != null)
                            lstRecipient.AddRange(arrPMWUsers);
                        break;
                    default:
                        break;
                }
            }
            lstRecipient.RemoveAll(item => item == 0);
            lstRecipient = lstRecipient.Distinct().ToList();
            return lstRecipient;
        }
    }
}
