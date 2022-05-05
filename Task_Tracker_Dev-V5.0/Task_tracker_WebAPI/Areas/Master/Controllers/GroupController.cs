using Common_Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Task_Tracker_CommonLibrary.DomainModels;
using Task_Tracker_CommonLibrary.Entity;
using Task_Tracker_CommonLibrary.Others;
using Task_Tracker_CommonLibrary.Utility;
using Task_Tracker_Library.Repository;
using Task_tracker_WebAPI.Controllers;

namespace Task_tracker_WebAPI.Areas.Master.Controllers
{
    public class GroupController : BaseAPIController
    {
        [HttpPost]
        public IList<GroupDM> GetAllGroups([FromBody] GroupMaster group)
        {
            IList<GroupDM> mList = null;
            try
            {
                mList = new List<GroupDM>();
                using (var uow = new UnitOfWork())
                {
                    mList = uow.GroupRepo.GetGroupsByEmployee(group.logged_in_user);
                    for (int i = 0; i < mList.Count; i++)
                    {
                        mList[i].RecordHasWriteAccess = uow.AccessControlRepo.returnGroupAccess(group.logged_in_user, mList[i].group_syscode);
                               
                    }
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetAllGroups", "GroupController");
            }


            return mList;
        }

        [HttpPost]
        public GroupDM GetGroupByID([FromBody] GroupDM grpDM)
        {
            GroupMaster grp = null;
            try
            {
                int logged_in_user = grpDM.logged_in_user;
                grp = new GroupMaster();
                using (var uow = new UnitOfWork())
                {
                    grp = uow.GroupRepo.GetList(x => x.is_deleted == false && x.group_syscode.Equals(grpDM.group_syscode), x => x.lstGroupMembers)?.FirstOrDefault();
                    if (grp != null)
                    {
                        grpDM = grp.Map<GroupMaster, GroupDM>();
                    }
                    grpDM.arrGrpHeadSyscodes = grp.lstGroupMembers.Where(x => x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head && !x.is_deleted).Select(x => x.employee_syscode).ToArray();
                    grpDM.arrMemSyscodes = grp.lstGroupMembers.Where(x => x.role_syscode == (int)Enum_Master.UserRoleEnum.Member && !x.is_deleted).Select(x => x.employee_syscode).ToArray();
                    grpDM.lstGroupMembers = null;
                    uow.CommonRepo.fillDDLdata(grpDM.ddlData);

                    grpDM.PageHasWriteAccess = uow.AccessControlRepo.returnGroupAccess(logged_in_user, grpDM.group_syscode);
                    grpDM.ddlData.opStatus = true;
                    grpDM.opStatus = true;
                }                
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetGroupByID", "GroupController");
                grpDM.opStatus = false;
                grpDM.ddlData.opStatus = false;
            }

            return grpDM;
        }

        [HttpPost]
        public GroupMaster PostGroup([FromBody] GroupDM _groupdm)
        {

            if (string.IsNullOrEmpty(_groupdm.group_name))
            {
                throw new Exception("Name of the group can not be left blank.");
            }
            if(string.IsNullOrEmpty(_groupdm.group_description))
            {
                throw new Exception("Description of the group can not be left blank.");
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {
                    GroupMaster grp = uow.GroupRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.group_name.Equals(_groupdm.group_name));
                    if (grp != null)
                    {
                        throw new Exception("Group with name " + _groupdm.group_name + " already exists in the database.");
                    }
                    grp = new GroupMaster();// = _groupdm as GroupMaster;//_groupdm.Map<GroupDM, GroupMaster>();
                    grp.group_name = _groupdm.group_name;
                    grp.group_description = _groupdm.group_description;
                    grp.group_email_id = _groupdm.group_email_id;
                    grp.is_active = _groupdm.is_active;
                    grp.is_deleted = false;
                    grp.created_by = _groupdm.logged_in_user;
                    grp.created_on = DateTime.Now;
                    uow.GroupRepo.Add(grp);
                    uow.commitTT();

                    AddUpdateMembers(_groupdm, uow, grp);

                    GroupMember gm = new GroupMember();
                    gm.group_syscode = grp.group_syscode;
                    gm.employee_syscode = _groupdm.logged_in_user;
                    gm.role_syscode = (int)Enum_Master.UserRoleEnum.Group_Creator;
                    gm.is_active = true;
                    gm.is_deleted = false;
                    gm.created_by = _groupdm.logged_in_user;
                    gm.created_on = DateTime.Now;
                    uow.GroupMemberRepo.Add(gm);

                    uow.commitTT();

                    od.opStatus = true;
                }
                if (od.opStatus)
                {
                    _groupdm.opStatus = true;
                    _groupdm.opMsg = od.opMsg;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", _groupdm.created_by.ToString(), "PostGroup", "GroupController");
                _groupdm.opStatus = false;
                _groupdm.opMsg = "Error: " + ex.Message;
                _groupdm.opInnerException = ex;
            }
            return _groupdm;
        }

        private void AddUpdateMembers(GroupDM _groupdm, UnitOfWork uow, GroupMaster grp)
        {
            List<GroupMember> gm, lstNewHeadMem, lstDelHeadMem, lstNewMem, lstDelMem;
            gm = uow.GroupMemberRepo.GetList(x => x.group_syscode == grp.group_syscode && x.role_syscode == (int)Enum_Master.UserRoleEnum.Group_Head && !x.is_deleted).ToList();

            lstNewHeadMem = new List<GroupMember>();
            lstDelHeadMem = new List<GroupMember>();
            getMembersList(_groupdm.logged_in_user, grp.group_syscode, Enum_Master.UserRoleEnum.Group_Head, gm, _groupdm.arrGrpHeadSyscodes, out lstNewHeadMem, out lstDelHeadMem);
            uow.GroupMemberRepo.AddRange(lstNewHeadMem);
            uow.GroupMemberRepo.UpdateRange(lstDelHeadMem);

            gm = uow.GroupMemberRepo.GetList(x => x.group_syscode == grp.group_syscode && x.role_syscode == (int)Enum_Master.UserRoleEnum.Member && !x.is_deleted).ToList();

            lstNewMem = new List<GroupMember>();
            lstDelMem = new List<GroupMember>();
            getMembersList(_groupdm.logged_in_user, grp.group_syscode, Enum_Master.UserRoleEnum.Member, gm, _groupdm.arrMemSyscodes, out lstNewMem, out lstDelMem);
            uow.GroupMemberRepo.AddRange(lstNewMem);
            uow.GroupMemberRepo.UpdateRange(lstDelMem);
        }

        private void getMembersList(int loggedInEmpSyscode, int groupSyscode, Enum_Master.UserRoleEnum role, List<GroupMember> gm, int[] arrMemSyscodes, out List<GroupMember> _lstNewMem, out List<GroupMember> _lstDelMem)
        {
            int[] arrNewMemSyscodes = { };
            _lstDelMem = new List<GroupMember>();
            _lstNewMem = new List<GroupMember>();

            if (gm == null || gm.Count <= 0)
            {
                arrNewMemSyscodes = arrMemSyscodes;
            }
            else
            {
                _lstDelMem = gm.Where(x => !arrMemSyscodes.Contains(x.employee_syscode)).ToList();
                arrNewMemSyscodes = arrMemSyscodes.Where(x => !gm.Any(y => y.employee_syscode == x)).ToArray();
            }
            _lstDelMem.ForEach(x => { x.is_active = false; x.is_deleted = true; x.modified_by = loggedInEmpSyscode; x.modified_on = DateTime.Now; });

            foreach (var mem in arrNewMemSyscodes)
            {
                GroupMember gp = new GroupMember();
                gp.group_syscode = groupSyscode;
                gp.employee_syscode = mem;
                gp.role_syscode = (int)role;
                gp.is_active = true;
                gp.is_deleted = false;                
                gp.created_by = loggedInEmpSyscode;
                gp.created_on = DateTime.Now;
                _lstNewMem.Add(gp);
            }
        }

        [HttpPut]
        public GroupMaster PutGroup([FromBody] GroupDM _groupdm)
        {
            if (_groupdm.group_syscode == 0)
            {
                throw new Exception("Invalid Group. Group Syscode not found in the request.");
            }
            if (string.IsNullOrEmpty(_groupdm.group_name))
            {
                throw new Exception("Name of the group can not be left blank.");
            }
            if (string.IsNullOrEmpty(_groupdm.group_description))
            {
                throw new Exception("Description of the group can not be left blank.");
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {
                    GroupMaster grpNameDup = uow.GroupRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.group_name.Equals(_groupdm.group_name));
                    if (grpNameDup != null && grpNameDup.group_syscode != _groupdm.group_syscode)
                    {
                        throw new Exception("Group with name " + _groupdm.group_name + " already exists in the database.");
                    }

                    GroupMaster grp = uow.GroupRepo.GetList(x => x.group_syscode == _groupdm.group_syscode && !x.is_deleted).FirstOrDefault();
                    if (grp == null)
                        throw new Exception("Group not found in the database for editing. Please confirm if it is not deleted from other source");

                    grp.group_name = _groupdm.group_name;
                    grp.group_description = _groupdm.group_description;
                    grp.group_email_id = _groupdm.group_email_id;
                    grp.is_active = _groupdm.is_active;
                    grp.modified_by = _groupdm.logged_in_user;
                    grp.modified_on = DateTime.Now;

                    uow.GroupRepo.Update(grp);
                    AddUpdateMembers(_groupdm, uow, grp);

                    uow.commitTT();
                    od.opStatus = true;
                }

                if (od.opStatus)
                {
                    _groupdm.opStatus = true;
                    _groupdm.opMsg = "Record updated successfully.";
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", _groupdm.created_by.ToString(), "PutGroup", "GroupController");
                _groupdm.opStatus = false;
                _groupdm.opMsg = ex.Message;
                _groupdm.opInnerException = ex;
            }
            return _groupdm;
        }


        [HttpPost]
        public GroupMaster AddUpdateGroupMembers([FromBody] IList<GroupMember> lstGrpMembers)
        {
            GroupMaster objGroup = new GroupMaster();

            if (lstGrpMembers.Count == 0)
            {
                objGroup.opStatus = false;
                objGroup.opMsg = "Invalid request. list of members in the request are 0";
                return objGroup;
            }
            try
            {
                using (var uow = new UnitOfWork())
                {

                    foreach (var member in lstGrpMembers)
                    {
                        if (member.group_member_syscode > 0)
                            uow.GroupMemberRepo.Update(member);
                        else
                            uow.GroupMemberRepo.Add(member);
                    }

                    uow.commitTT();
                    objGroup.opStatus = true;
                    objGroup.lstGroupMembers = lstGrpMembers; //since this entity object is directly tracked by the context, its primary key gets updated.                   
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", "", "AddUpdateGroupMembers", "GroupController");
                objGroup.opStatus = false;
                objGroup.opMsg = ex.Message;
            }
            return objGroup;
        }

    }
}
