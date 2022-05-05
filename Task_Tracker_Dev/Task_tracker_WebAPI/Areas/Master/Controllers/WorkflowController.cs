using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Interface;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Master.Controllers
{
    public class WorkflowController : BaseAPIController
    {
        [HttpPost]
        public IList<WorkflowMaster> GetAllWorkFlowList([FromBody] WorkflowMaster workflow)
        {
            IList<WorkflowMaster> mList = null;
            try
            {
                mList = new List<WorkflowMaster>();
                using (var uow = new UnitOfWork())
                {
                    mList = uow.WorkflowRepo.GetList(x => x.is_deleted == false && x.group_syscode.Equals(workflow.group_syscode)).ToList(); // && x.workflow_syscode.Equals(workflowID) , x => x.lstModules 
                    var empNameDic = uow.EmployeeRepo.GetEmpNamesBySyscode(mList.Select(x => x.created_by).Distinct().ToList());

                    if (empNameDic == null || empNameDic.Count <= 0) empNameDic = new Dictionary<int, string>();

                    for (int i = 0; i < mList.Count; i++)
                    {
                        int creator = mList[i].created_by;
                        mList[i].created_by_Name = empNameDic.ContainsKey(creator) ? empNameDic[creator] : string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetAllWorkFlowList", "WorkflowController");
            }


            return mList;
        }

        [HttpPost]
        public OperationDetailsDTO GetWorkFlowByID([FromBody] WorkflowMaster workflow)
        {
            WorkflowDM wfDM = null;
            OperationDetailsDTO od = null;
            try
            {
                od = new OperationDetailsDTO();
                wfDM = new WorkflowDM();
                using (var uow = new UnitOfWork())
                {
                   workflow = uow.WorkflowRepo.GetList(x => x.is_deleted == false && x.workflow_syscode.Equals(workflow.workflow_syscode), x => x.lstWFLevels)?.FirstOrDefault();
                    if (workflow == null)
                        throw new Exception("Error occured while fetching workflow data.");

                    wfDM = workflow.Map<WorkflowMaster, WorkflowDM>();

                    if (wfDM == null) throw new Exception("Error occured while mapping WorkflowMaster to WorkflowDM");

                    if (wfDM.lstWFLevels != null)
                        wfDM.lstWFLevels = wfDM.lstWFLevels.OrderBy(x => x.level_order).ToList();

                    wfDM.module_count = uow.TasksRepo.getCountModuleByWorkflow(workflow.workflow_syscode);

                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetWorkFlowByID", "WorkflowController");
            }

            return wfDM;
        }

        [HttpPost]
        public WorkflowMaster PostWorkflow([FromBody] WorkflowMaster workflow)
        {

            if (string.IsNullOrEmpty(workflow.workflow_name))
            {
                workflow = new WorkflowMaster();
                workflow.opStatus = false;
                workflow.opMsg = "Invalid workflow";
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO(); ;
                using (var uow = new UnitOfWork())
                {
                    WorkflowMaster wf = uow.WorkflowRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.workflow_name.Equals(workflow.workflow_name));
                    if (wf != null)
                    {
                        throw new Exception("Workflow with name " + workflow.workflow_name + " already exists in the database.");
                    }

                    uow.WorkflowRepo.Add(workflow);//saveOperation(workflow, System.Data.Entity.EntityState.Added); 
                    uow.commitTT();
                    od.opStatus = true;
                }
                if (od.opStatus)
                {
                    workflow.opStatus = true;
                    workflow.opMsg = od.opMsg;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", workflow.created_by.ToString(), "PostWorkflow", "WorkflowController");
                workflow.opStatus = false;
                workflow.opMsg = "Exception Occurred! " + ex.Message ;
                workflow.opInnerException = ex;
            }
            return workflow;
        }


        [HttpPut]
        public WorkflowMaster PutWorkflow([FromBody] WorkflowMaster workflow)
        {
            if (workflow.workflow_syscode == 0)
            {
                workflow = new WorkflowMaster();
                workflow.opStatus = false;
                workflow.opMsg = "Invalid workflow";
                return workflow;
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {
                    WorkflowMaster wf = uow.WorkflowRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.workflow_name.Equals(workflow.workflow_name));
                    if (wf != null)
                    {
                        throw new Exception("Workflow with name " + workflow.workflow_name + " already exists in the database.");
                    }
                    uow.WorkflowRepo.Update(workflow);//saveOperation(workflow, System.Data.Entity.EntityState.Modified);
                    uow.commitTT();
                    od.opStatus = true;
                }

                if (od.opStatus)
                {
                    workflow.opStatus = true;
                    workflow.opMsg = "Record updated successfully.";
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", workflow.created_by.ToString(), "PutWorkflow", "WorkflowController");
                workflow.opStatus = false;
                workflow.opMsg = ex.Message;//"Exception Occurred!";
                workflow.opInnerException = ex.InnerException;
            }
            return workflow;
        }


        [HttpDelete]
        public WorkflowMaster DeleteWorkflow([FromBody] WorkflowMaster workflow)
        {

            if (workflow.workflow_syscode == 0)
            {
                workflow = new WorkflowMaster();
                workflow.opStatus = false;
                workflow.opMsg = "Invalid workflow";
            }
            try
            {
                using (var uow = new UnitOfWork())
                {
                    uow.WorkflowRepo.Update(workflow);//saveOperation(workflow, System.Data.Entity.EntityState.Deleted); 
                    uow.commitTT();
                    workflow.opStatus = true;
                    workflow.opMsg = "Successfully deleted the record.";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", workflow.created_by.ToString(), "PutWorkflow", "WorkflowController");
                workflow.opStatus = false;
                workflow.opMsg = ex.Message;
                workflow.opInnerException = ex;
            }
            return workflow;
        }


        [HttpPost]
        public WorkflowMaster AddUpdateWorkflowLevels([FromBody] WorkflowDM wfDM )//IList<WorkflowLevelDetails> workflowLevels
        {
            WorkflowMaster objWorkflow = new WorkflowMaster();
            IList<WorkflowLevelDetails> workflowLevels = wfDM.lstWFLevels;

            if (workflowLevels.Count == 0)
            {
                objWorkflow.opStatus = false;
                objWorkflow.opMsg = "Invalid workflow";
                return objWorkflow;
            }
            try
            {
                using (var uow = new UnitOfWork())
                {
                    //OperationDetailsDTO od = new OperationDetailsDTO();

                    foreach (var level in workflowLevels)
                    {
                       
                        if (level.level_syscode > 0 && wfDM.module_count == 0)
                        {
                            uow.WorkflowLevelRepo.Update(level);
                        }
                        else if (level.level_syscode > 0 && wfDM.module_count > 0)
                        {
                            continue;
                        }
                        else
                            uow.WorkflowLevelRepo.Add(level);                                              
                    }

                    uow.commitTT();
                    //od.opStatus = true;
                    objWorkflow.opStatus = true;
                    objWorkflow.lstWFLevels = workflowLevels; //since this entity object is directly tracked by the context, its primary key gets updated.
                 
                    //if (od.opStatus)
                    //{
                    //    level.opStatus = true;
                    //    if (objWorkflow.lstWFLevels == null)
                    //        objWorkflow.lstWFLevels = new List<WorkflowLevelDetails>();

                    //    objWorkflow.lstWFLevels.Add(level);
                    //    objWorkflow.opStatus = true;
                    //    objWorkflow.opMsg = od.opMsg;
                    //}
                    //else throw new Exception(od.opMsg);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", "", "AddUpdateWorkflowLevels", "WorkflowController");
                objWorkflow.opStatus = false;
                objWorkflow.opMsg = ex.Message;
            }
            return objWorkflow;
        }

        [HttpPost]
        public WorkflowMaster CopyWorkflow([FromBody] WorkflowMaster workflow)
        {
            WorkflowDM wfDM = null;
            WorkflowMaster WFnew = null;
            OperationDetailsDTO od = null;
            string WFnew_name = null;

            try
            {
                od = new OperationDetailsDTO();
                wfDM = new WorkflowDM();
                WFnew_name = workflow.workflow_name + "_2";

                if(workflow.workflow_syscode <= 0)
                {
                    WFnew = new WorkflowMaster();
                    throw new Exception("Invalid parent workflow.");
                }
                if (string.IsNullOrEmpty(workflow.workflow_name))
                {
                    WFnew = new WorkflowMaster();
                    throw new Exception("Parent workflow name cannot be empty.");
                }
                using (var uow = new UnitOfWork())
                {
                    List<WorkflowLevelDetails> lstWFLevel = workflow.lstWFLevels.ToList();
                    WorkflowMaster wf = uow.WorkflowRepo.GetSingle(x => x.is_deleted == false
                                                                     && x.workflow_name.Equals(WFnew_name));
                    if (wf != null)
                    {
                        WFnew = new WorkflowMaster();
                        throw new Exception("Workflow with name "+ WFnew_name + " already exists in the database.");
                    }

                    //workflow = uow.WorkflowRepo.GetList(x => x.is_deleted == false && x.workflow_syscode.Equals(workflow.workflow_syscode), x => x.lstWFLevels)?.FirstOrDefault();
                    //if (workflow == null)
                    //    throw new Exception("Error occured while fetching workflow data.");

                    WFnew = new WorkflowMaster();
                    WFnew.created_by = workflow.created_by;
                    WFnew.workflow_name = WFnew_name;
                    WFnew.is_active = true;
                    WFnew.is_deleted = false;
                    WFnew.group_syscode = workflow.group_syscode;

                    uow.WorkflowRepo.Add(WFnew);
                    uow.commitTT();

                    

                    lstWFLevel.ForEach(x => { x.workflow_syscode = WFnew.workflow_syscode;  x.is_deleted = false; x.is_active = true; x.created_by = WFnew.created_by; });

                    uow.WorkflowLevelRepo.AddRange(lstWFLevel);

                    uow.commitTT();
                    WFnew.opStatus = true;
                    WFnew.opMsg = "Workflow copied successfully.";
                    //wfDM = workflow.Map<WorkflowMaster, WorkflowDM>();

                    //if (wfDM == null) throw new Exception("Error occured while mapping WorkflowMaster to WorkflowDM");
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "CopyWorkFlow", "WorkflowController");
                WFnew.opStatus = false;
                WFnew.opMsg = ex.Message;
            }

            return WFnew;
        }

    }
}
