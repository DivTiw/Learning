using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Task_Tracker_Solution.Utility
{
    public static class cWebApiNames
    {
        #region "workflow"

        public const string APIGetAllWorkflowList = "api/Workflow/GetAllWorkFlowList";
        public const string APIPostWorkflow = "api/Workflow/PostWorkflow";
        public const string APIGetWorkflowByID = "api/Workflow/GetWorkflowByID";
        public const string APIUpdateWorkflow = "api/Workflow/PutWorkflow";
        public const string APIAddUpdateWorkflowLevels = "api/Workflow/AddUpdateWorkflowLevels";
        public const string APICopyWorkflow = "api/Workflow/CopyWorkflow";

        #endregion

        #region "Project"
        public const string APIGetAllProjectList = "api/Project/GetAllProjectList";
        public const string APIGetProjectByID = "api/Project/GetProjectByID";
        public const string APIPostProject = "api/Project/PostProject";
        public const string APIUpdateProject = "api/Project/PutProject";
        public const string APIGetProjectDetails = "api/Project/GetProjectDetails";
        public const string APIAddUpdateProjectDetails = "api/Project/AddUpdateProjectDetails";

        #endregion


        #region "Module"
        public const string APIAddUpdateProjectModules = "api/Module/AddUpdateProjectModules";
        public const string APIPostModule = "api/Module/PostModule";
        public const string APIPutModule = "api/Module/PutModule";
        public const string APIGetProjectModuleList = "api/Module/GetModuleList";
        public const string APIGetModuleByID = "api/Module/GetModuleByID";
        #endregion

        #region "ModuleWFLevelMap"

        public const string APIGetModulesListByProject = "api/ModuleWFLevelMap/GetModulesListByProject";
        public const string APIAddWFLevelMapping = "api/ModuleWFLevelMap/AddWFLevelMapping";
        public const string GetLevelTaskUsersList = "api/ModuleWFLevelMap/GetLevelTaskUsersList";

        #endregion

        #region "Common"
        public const string APIGetDDLData = "api/Common/GetDDLData";
        #endregion

        #region "Task"
        public const string APIGetMyTasks = "api/Task/MyTasks";
        public const string APIViewTaskByID = "api/Task/ViewTaskByID";
        public const string APIGetMyCreatedTasks = "api/Task/MyCreatedTasks";
        public const string APIGetMyOwnedTasks = "api/Task/MyOwnedTasks";

        public const string APISaveTaskTrail = "api/Task/SaveTrail";
        public const string APISaveTask = "api/Task/SaveTask";

        public const string APISaveTaskStart = "api/Task/SaveTaskStart";
        public const string APISaveTaskStop = "api/Task/SaveTaskStop";

        public const string APIGetTaskReport = "api/Task/GetTaskReport";
        #endregion

        #region "ProjectTask"
        public const string APIGetProjectTaskInfo = "api/ProjectTask/GetProjectTaskInfo";
        public const string APISaveProjectTask = "api/ProjectTask/SaveProjectTask";
        #endregion

        #region "SetWeightage"

        public const string APIGetTaskSubTasks = "api/Task/GetTask_SubTasks";
        public const string APISaveWeightage = "api/Task/SaveWeightage";
        #endregion

        #region "Category"
        public const string APIGetAllCategoryList = "api/Category/GetAllCategoryList";
        public const string APIGetCategoryByID = "api/Category/GetCategoryByID";
        public const string APIPostCategory = "api/Category/PostCategory";        
        public const string APIUpdateCategory = "api/Category/PutCategory";
        #endregion

      
        #region Attachments
        public const string AppDownloadAttachments = "/Services/FileService/DownloadFile";
        public const string APIDownloadAttachments = "api/FileService/DownloadFile";
        #endregion

        #region "Dashboard"

        public const string APIGetTodaysLatestTasks = "api/Dashboard/GetTodaysLatestTasks";
        public const string APIGetWorkingUsers = "api/Dashboard/GetWorkingUsers";
        public const string APIGetDashboard = "api/Dashboard/GetDashboard";
        public const string APIGetTeamActivity = "api/Dashboard/GetTeamActivity";
        public const string APIGetMyTaskActivity = "api/Dashboard/GetMyTaskActivity";
        #endregion

        #region "Group"

        public const string APIGetAllGetAllGroups = "api/Group/GetAllGroups";
        public const string APIGetGroupByID = "api/Group/GetGroupByID";
        public const string APIPostGroup = "api/Group/PostGroup";
        public const string APIUpdateGroup = "api/Group/PutGroup";
        public const string APIAddUpdateGroupMembers = "api/Group/AddUpdateGroupMembers";

        #endregion

        #region "Release Report"

        public const string APIGetTaskReleaseReport = "api/Reports/GetTaskReleaseReport";

        public const string APIUpdateRelease = "api/Reports/UpdateRelease";

        public const string APIGetReleaseDetails = "api/Reports/GetReleaseDetails";


        #endregion

    }
}