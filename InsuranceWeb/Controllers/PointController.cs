using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsuranceWeb.Model;
using InsuranceWeb.Model.ViewModel;
using InsuranceWeb.EFRepository.Repository;
using Insurance.Business.Services;

namespace InsuranceWeb.Controllers
{
    public class PointController : Controller
    {
        //. 構造函數
        private InsurService _insurService;
        private InsurePointEFRepository _pointRepo;

        public PointController()
        {
            _insurService = new InsurService();
            _pointRepo = new InsurePointEFRepository();
        }

        public ActionResult Index()
        {
            return View();
        }

        //. Get 新增點數
        [HttpGet]
        public ActionResult InsertPoint()
        {
            InsertViewModel newPoint = new InsertViewModel();
            return View(newPoint);
        }
 
        /// <summary>
        /// Post 新增點數
        /// </summary>
        /// <param name="newPoint">輸入點數資料</param>
        /// <returns></returns>
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult InsertPoint(InsertViewModel newPoint)
        {
            var res = new
            {
                Code = "",
                Message = "",
            };
            //如果驗證失敗 回傳錯誤訊息
            if (!ModelState.IsValid)
            {
                var errorMessage = ModelState.
                                    Values.
                                    SelectMany(v => v.Errors)
                                    .Select(e => e.ErrorMessage);
                res = new
                {
                    Code = "0001",
                    Message = string.Join("，", errorMessage),
                };
                return Json(res);
            }
            //執行給值與寫入方法
            var response = _insurService.giveValueAndWrite(newPoint);
            //若是新增成功 回傳正確
            res = response.IsSuccess == true ? new { Code = "0000", response.Message, }
            : new { Code = "0001", response.Message, };

            return Json(res);

        }

        //. Get 搜尋點數頁面
        [HttpGet]
        public ActionResult Search()
        {
            GetBaseDropdownListModel();
            return View();
        }

        //. DropdownList資料
        private void GetBaseDropdownListModel()
        {
            var items = new List<SelectListItem>();
            items.Add(new SelectListItem()
            {
                Text = "點數來源(非必填)",
                Value = null,
                Selected = true
            });
            items.Add(new SelectListItem()
            {
                Text = "投保",
                Value = "1"
            });
            items.Add(new SelectListItem()
            {
                Text = "後台抽獎",
                Value = "2"
            });
            items.Add(new SelectListItem()
            {
                Text = "兌換",
                Value = "3"
            });

            Session["TypeList"] = items;
        }

        /// <summary>
        /// Post搜尋點數資料
        /// </summary>
        /// <param name="AppId">UserID</param>
        /// <param name="Search_AcType">來源平台(非必填)</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string AppId, int? Search_AcType)
        {
            List<InsurePoint> resultList = _pointRepo.SearchPoint(AppId, Search_AcType);
            if (resultList.Count == 0)
            {
                return Json(new { IsSuccess = false, Code = "0001", Message = "查無資料" });
            }
            else
            {
                //所有點數：依點數LOG Table統計客戶點數。
                ViewBag.sum = _insurService.PointSum(AppId);
                return PartialView("_QueryResult", resultList);
            }//return Helper.PointHelper.AppliSearchResult(resultList, PartialView("_QueryResult", resultList));

        }
        
        /// <summary>
        /// Post 搜尋Log
        /// </summary>
        /// <param name="AppId">UserID</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SearchLog(string AppId)
        {
            List<InsurePointLog> resultList = _pointRepo.SearchPointLog(AppId);
            if (resultList.Count == 0)
            {
                return Json(new { IsSuccess = false, Code = "0001", Message = "查無資料" });
            }
            else
            {
                return PartialView("_LogQueryResult", resultList);
            }
        }

        /// <summary>
        /// Post 註銷點數
        /// </summary>
        /// <param name="id">該筆點數ID</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CancelPoint(int id)
        {
            //註銷是做Updata 不是Delete
            var res = _insurService.CancelPointAndUpdate(id);
            if (res.IsSuccess)
            {
                return Json(new { res.IsSuccess, Code = "0000", res.Message });
            }
            else
                return Json(new { res.IsSuccess, Code = "0001", res.Message });
        }

        //. Get 兌換點數
        [HttpGet]
        public ActionResult ExchangePoint()
        {
            return View();
        }

        /// <summary>
        /// Post 兌換點數
        /// </summary>
        /// <param name="AppId">UserID</param>
        /// <param name="ExPoint">兌換點數數量</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ExchangePoint(string AppId, int ExPoint)
        {
            var res = _insurService.ExchangePointAndWLog(AppId, ExPoint);
            if (res.IsSuccess)
            {
                return Json(new { res.IsSuccess, Code = "0000", res.Message });
            }
            else
                return Json(new { res.IsSuccess, Code = "0001", res.Message });
        }

        //. 統合搜尋
        [HttpPost]
        public ActionResult BothSearch(InsertViewModel newPoint)
        {
            return PartialView("_QueryResult", _pointRepo.SearchPoint(newPoint.ApplicantId, null));
        }


    }
}