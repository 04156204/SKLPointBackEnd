using InsuranceWeb.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceWeb.EFRepository.Repository
{
    public class InsurePointEFRepository
    {


        /// <summary>
        /// 新增點數
        /// </summary>
        /// <param name="insurePoint"></param>
        /// <returns>DB是否異動</returns>
        public int AddPoint(InsurePoint insurePoint)
        {
            int res = 0;
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                //context.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[InsurePoint] ON");
                context.InsurePoint.Add(insurePoint);
                res = context.SaveChanges();
            }
            return res;
        }
        /// <summary>
        /// 查詢點數
        /// </summary>
        /// <param name="AppId"></param>
        /// <param name="AcType"></param>
        /// <returns></returns>
        public List<InsurePoint> SearchPoint(string AppId, int? AcType)
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                //0716 問羽翔這邊有沒有簡潔的寫法
                if (AcType == null)
                {
                    var pointData = context.InsurePoint
                                    .Where(p => p.ApplicantId == AppId);
                    return pointData.ToList();
                }
                else
                {
                    var pointData = context.InsurePoint
                                    .Where(p => p.ApplicantId == AppId && p.ActionType == AcType);
                    return pointData.ToList();
                }
            }
        }
        /// <summary>
        /// 查詢點數Log
        /// </summary>
        /// <param name="AppId">客戶Id</param>
        /// <returns></returns>
        public List<InsurePointLog> SearchPointLog(string AppId)
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                var pointData = context.InsurePointLog
                                .Where(p => p.ApplicantId == AppId);
                return pointData.ToList();
            }
        }

        /// <summary>
        /// 寫Log
        /// </summary>
        /// <param name="insureLog"></param>
        /// <returns></returns>
        public static void WriteLog(InsurePointLog insureLog)
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                context.InsurePointLog.Add(insureLog);
                context.SaveChanges();
            }
        }
        /// <summary>
        /// 更新資料庫
        /// </summary>
        /// <param name="insurePoint"></param>
        /// <returns></returns>
        public int UpdatePoint(InsurePoint insurePoint)
        {
            int res = 0;
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                context.Entry(insurePoint).State = System.Data.Entity.EntityState.Modified;
                res = context.SaveChanges();
            }
            return res;
        }

        public List<InsurePoint> GetAll()
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                return context.InsurePoint.ToList();
            }
        }
        /// <summary>
        /// 由流水號取資料(唯一一筆)
        /// </summary>
        /// <param name="id">流水號</param>
        /// <returns></returns>
        public InsurePoint GetById(int id)
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                var allPoint = GetAll();
                var returnPoint = allPoint.FirstOrDefault(point => point.Id == id);
                return returnPoint;
            }
        }


        //被棄用的新增流水號
        public int GetCurrentId(string tableN)
        {
            using (InsuranceWebEntities context = new InsuranceWebEntities())
            {
                if (tableN == "InsurePoint")
                {
                    var queryStudent = context.InsurePoint //希望可以弄成變數 答：設Base
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault();
                    return queryStudent.Id + 1;
                }
                else
                {
                    var queryStudent = context.InsurePointLog
                        .OrderByDescending(p => p.Id)
                        .FirstOrDefault();
                    return queryStudent.Id + 1;
                }
            }
        }






    }
}
