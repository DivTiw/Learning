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
    public class CategoryController : BaseAPIController
    {
        [HttpPost]
        public IList<CategoryMaster> GetAllCategoryList([FromBody] CategoryMaster category)
        {
            IList<CategoryMaster> mList = null;
            try
            {
                mList = new List<CategoryMaster>();
                using (var uow = new UnitOfWork())
                {
                    mList = uow.CategoryRepo.GetList(x => !x.is_deleted && x.group_syscode.Equals(category.group_syscode)).ToList();
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
                Log.LogError(ex.Message, "", null, "GetAllCategoryList", "CategoryController");
            }

            return mList;
        }

        [HttpPost]
        public CategoryDM GetCategoryByID([FromBody] CategoryMaster category)
        {
            CategoryDM tcm = null;
            try
            {
                tcm = new CategoryDM();
                using (var uow = new UnitOfWork())
                {
                    tcm = uow.CategoryRepo.GetList(x=> x.category_syscode == category.category_syscode).Select(x=> new CategoryDM { category_name = x.category_name, category_syscode = x.category_syscode, is_active = x.is_active}).FirstOrDefault();//uow.TaskCategoryRepo.GetList(x => x.is_deleted == false && x.category_syscode.Equals(category.category_syscode), x => x.lstWFLevels)?.FirstOrDefault();
                   
                    //tcm.ddlData.Predicate[DBTableNameEnums.vw_department_master]["GetData"] = true;
                    //uow.CommonRepo.fillDDLdata(tcm.ddlData);
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", null, "GetCategoryByID", "CategoryController");
            }

            return tcm;
        }

        [HttpPost]
        public CategoryMaster PostCategory([FromBody] CategoryMaster Category)
        {

            if (string.IsNullOrEmpty(Category.category_name))
            {
                Category = new CategoryMaster();
                Category.opStatus = false;
                Category.opMsg = "Invalid workflow";
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO(); ;
                using (var uow = new UnitOfWork())
                {
                    CategoryMaster cat = uow.CategoryRepo.GetSingle(x => x.is_deleted == false
                                                                      && x.category_name.Equals(Category.category_name)
                                                                      && x.group_syscode == Category.group_syscode);
                    if (cat != null)
                    {
                        throw new Exception("Category with name " + Category.category_name + " already exists in this Group.");
                    }

                    uow.CategoryRepo.Add(Category);//saveOperation(Category, System.Data.Entity.EntityState.Added); 
                    uow.commitTT();
                    od.opStatus = true;
                }
                if (od.opStatus)
                {
                    Category.opStatus = true;
                    Category.opMsg = od.opMsg;
                }
            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", Category.created_by.ToString(), "PostCategory", "CategoryController");
                Category.opStatus = false;
                Category.opMsg = "Exception Occurred! " + ex.Message;
                Category.opInnerException = ex;
            }
            return Category;
        }


        [HttpPut]
        public CategoryMaster PutCategory([FromBody] CategoryMaster Category)
        {
            if (Category.category_syscode == 0)
            {
                Category = new CategoryMaster();
                Category.opStatus = false;
                Category.opMsg = "Invalid Category";
                return Category;
            }
            try
            {
                OperationDetailsDTO od = new OperationDetailsDTO();
                using (var uow = new UnitOfWork())
                {
                    CategoryMaster cat = uow.CategoryRepo.GetSingle(x => x.is_deleted == false
                                                                     && x.category_name.Equals(Category.category_name)
                                                                     && x.group_syscode == Category.group_syscode);
                    if (cat != null)
                    {
                        throw new Exception("Category with name " + Category.category_name + " already exists in this Group.");
                    }
                    uow.CategoryRepo.Update(Category);//saveOperation(Category, System.Data.Entity.EntityState.Modified);
                    uow.commitTT();
                    od.opStatus = true;
                }

                if (od.opStatus)
                {
                    Category.opStatus = true;
                    Category.opMsg = "Record updated successfully.";
                }

            }
            catch (Exception ex)
            {
                Log.LogError(ex.Message, "", Category.created_by.ToString(), "PutCategory", "CategoryController");
                Category.opStatus = false;
                Category.opMsg = ex.Message;//"Exception Occurred!";
                Category.opInnerException = ex;
            }
            return Category;
        }

    }
}
