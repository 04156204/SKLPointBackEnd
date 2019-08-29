using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using InsuranceWeb.Model;
using InsuranceWeb.Model.ViewModel;
using InsuranceWeb.EFRepository.Repository;

namespace Insurance.Business.Services
{
    public class InsurService
    {
        private InsurePointEFRepository _pointRepo;

        public InsurService()
        {
            _pointRepo = new InsurePointEFRepository();
        }


        /* ActionType 1:投保 2:後台抽獎 3:兌換*/
        /* PointType  0:未確認是否給付點數 1:放棄點數 2:點數待核發 3:核發*/
        /*            4:註銷 5:已兌換 99:未符合資格-不處理*/

        /// <summary>
        /// 新增點數
        /// </summary>
        /// <param name="insurePoint"></param>
        /// <returns>RtnResult</returns>
        public RtnResult<InsurePoint> giveValueAndWrite(InsertViewModel insertData)
        {
            InsurePoint insurePoint = new InsurePoint();
            insurePoint.ApplicantId = insertData.ApplicantId;
            insurePoint.ActionType = 2;
            insurePoint.ActionContent = insertData.ActionContent;
            insurePoint.PlanCode = null;
            insurePoint.Point = insertData.Point;
            insurePoint.UsablePoint = UsablePointCount(insurePoint);
            insurePoint.PointType = 3;
            insurePoint.IssueDate = DateTime.Now;
            insurePoint.ExpiryDate = DateTime.Now.AddDays(365);
            insurePoint.Source = "SKEU2176";
            insurePoint.TransID = null;
            insurePoint.CreateDate = DateTime.Now;
            //insurePoint.UpdatedDate = DateTime.Now.AddMinutes(3);
            var repo = _pointRepo.AddPoint(insurePoint);
            changeToLogAndWrite(insurePoint, "後台手動新增", insurePoint.Point);

            return repo == 0 ?
                giveResult(1,"新增失敗") : giveResult(0, "新增成功，賀！");
        }

        /// <summary>
        /// 把InsurePointModel轉成Log架購並寫入Log
        /// </summary>
        /// <param name="insurePoint"></param>
        /// <param name="action"></param>
        public static void changeToLogAndWrite(InsurePoint insurePoint, string action, int point)
        {
            InsurePointLog newLog = new InsurePointLog();
            newLog.ApplicantId = insurePoint.ApplicantId;
            newLog.Content = action;
            newLog.Point = point; //註銷：傳進來已是減項
            newLog.CreateDate = DateTime.Now;//insurePoint.CreateDate ?? null;
            newLog.Memo = "";
            InsurePointEFRepository.WriteLog(newLog);
        }

        /// <summary>
        /// 依據LogTable加總點數
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public int PointSum(string Appid)
        {
            List<InsurePointLog> beSearched = _pointRepo.SearchPointLog(Appid);
            int sum = 0;
            foreach (var pointResault in beSearched)
            {
                sum += pointResault.Point;
            }
            return sum;
        }

        /// <summary>
        /// 註銷點數
        /// </summary>
        /// <param name="id">被註銷點數的流水號</param>
        /// <returns></returns>
        public RtnResult<InsurePoint> CancelPointAndUpdate(int id)
        {
            //用id取點數資料
            InsurePoint thePoint = _pointRepo.GetById(id);
            //調整資料為註銷狀態
            thePoint.UsablePoint = thePoint.UsablePoint - thePoint.Point;//不確定是不是該Appid的UsablePoint都減
            thePoint.PointType = 4; /*4:註銷*/
            thePoint.UpdatedDate = DateTime.Now;
            //更新資料庫
            var repo = _pointRepo.UpdatePoint(thePoint);
            //寫Log
            changeToLogAndWrite(thePoint, "退保-註銷點數", -thePoint.Point);

            return repo == 0 ?
           giveResult(1,"註銷失敗") : giveResult(0, "註銷成功！");
        }

        /// <summary>
        /// 兌換點數
        /// </summary>
        /// <param name="exPoint">輸入的兌換點數(正數)</param>
        /// <returns></returns>
        public RtnResult<InsurePoint> ExchangePointAndWLog(string AppId, int ExPoint)
        {
            /*擁有點數不夠換*/
            if (ExPoint > PointSum(AppId))
                return giveResult(1,"點數不足");
            /*夠換*/
            else
            {
                //寫InsurePoint資料庫
                var exPointDetail = new InsurePoint();
                exPointDetail.ApplicantId = AppId;
                exPointDetail.ActionType = 3;  /*3:兌換*/
                exPointDetail.ActionContent = "兌換PCHome點數" + ExPoint + "點";
                exPointDetail.PlanCode = null;
                exPointDetail.Point = -ExPoint;
                exPointDetail.UsablePoint = 0;
                exPointDetail.PointType = 5;  /*5:已兌換*/
                exPointDetail.IssueDate = DateTime.Now;//為何兌點有應核發日期？
                exPointDetail.ExpiryDate = null;
                exPointDetail.Source = "PCHome";
                exPointDetail.TransID = "L-XXXXX";
                exPointDetail.CreateDate = DateTime.Now;
                exPointDetail.UpdatedDate = null;
                var repo = _pointRepo.AddPoint(exPointDetail);
                //開始兌換 更新歷史點數的可用點數
                ReflashUsablePoint(exPointDetail);
                //寫Log
                changeToLogAndWrite(exPointDetail, "兌換P幣" + ExPoint + "點", -ExPoint);

                return repo == 0 ?
                           giveResult(1,"兌點失敗") : giveResult(0, "兌點成功！");
            }
        }

        public void ReflashUsablePoint(InsurePoint insurePoint)
        {
            
            /*兌點*/
            //1.找同一Appid資料
            List<InsurePoint> pointList = _pointRepo.SearchPoint(insurePoint.ApplicantId, null);
            int pointBeExchanged = insurePoint.Point;//負的
            //2.歷史點數UP是正
            foreach (var apoint in pointList)
            {
                //判斷可用點數是否為負
                if (apoint.UsablePoint > 0)
                {
                    //3.1 該筆歷史點數UP不夠換；會繼續找下一筆
                    if (pointBeExchanged + apoint.UsablePoint < 0)
                    {
                        pointBeExchanged += apoint.UsablePoint;//-7=-12+5 -2=-7+5
                        apoint.UsablePoint = 0; //該筆UsablePoint清零
                        apoint.UpdatedDate = insurePoint.CreateDate;
                        var repo = _pointRepo.UpdatePoint(apoint);
                    }
                    //3.2 該筆歷史點數UP剛好換完或有餘；結束foreach
                    else
                    {
                        apoint.UsablePoint += pointBeExchanged; //3=5+-2
                        pointBeExchanged = 0;
                        apoint.UpdatedDate = insurePoint.CreateDate;
                        var repo = _pointRepo.UpdatePoint(apoint);
                        break;
                    }
                }

            }

        }

        /// <summary>
        /// 計算可用點數
        /// </summary>
        /// <param name="thisPoint"></param>
        /// <returns></returns>
        public int UsablePointCount(InsurePoint thisPoint)
        {
            /*新增*/
            //由appid找取所有同客戶之點數
            List<InsurePoint> pointList1 = _pointRepo.SearchPoint(thisPoint.ApplicantId, null);
            int pointCount = thisPoint.Point;
            if (pointList1.Count == 0 )
            {
                return thisPoint.Point;
            }
            else
            {
                foreach (var apoint in pointList1)
                {
                    if (apoint.UsablePoint < 0) 
                    {
                        if (pointCount + apoint.UsablePoint >= 0)//  5-1  4-2  2-5
                        {
                            pointCount  += apoint.UsablePoint;// 4=5+-1  2=4+-2
                            apoint.UsablePoint = 0;
                            apoint.UpdatedDate = thisPoint.CreateDate;
                            var repo = _pointRepo.UpdatePoint(apoint);
                        }
                        else //
                        {
                            apoint.UsablePoint = pointCount + apoint.UsablePoint; // -3=2+-5
                            pointCount = 0;
                            apoint.UpdatedDate = thisPoint.CreateDate;
                            var repo = _pointRepo.UpdatePoint(apoint);
                            break;
                        }
                    }
                }

                return pointCount;
            }
            
        }


        /// <summary>
        /// 給予回傳值
        /// </summary>
        /// <param name="code"></param>
        /// <returns>RtnResult</returns>
        public RtnResult<InsurePoint> giveResult(int code,string mes)
        {
            return code == 0 ?
                new RtnResult<InsurePoint>() { IsSuccess = true, Message = mes} :
                new RtnResult<InsurePoint>() { IsSuccess = false, Message = mes };
        }




    }
}