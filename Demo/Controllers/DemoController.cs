using Dapper;
using Demo.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class DemoController : ApiController
    {
        private string _connectionStr = String.Format(@"Data Source={0}\{1}.db3;Version=3;", AppDomain.CurrentDomain.BaseDirectory, ConfigurationManager.AppSettings["dbName"]);

        /// <summary>
        /// 查詢
        /// </summary>
        /// <returns></returns>
        // GET api/Demo
        public IHttpActionResult Get()
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    var sql = @" SELECT * FROM Demo";
                    List<DemoModel> listData = conn.Query<DemoModel>(sql).ToList();
                    return Json(listData);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 條件查詢
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/Demo/5
        public IHttpActionResult Get(string id)
        {
            try
            {
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                   // conn.Open();
                    var sql = @" SELECT * FROM Demo WHERE ID=@ID";
                    List<DemoModel> listData = conn.Query<DemoModel>(sql, new { ID = id }).ToList();
                    return Json(listData);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // POST api/Demo
        public IHttpActionResult Post([FromBody] List<DemoModel> model)
        {
            try
            {
                var data = from m in model
                           where string.IsNullOrEmpty(m.NAME)
                           select m;
                if (data != null && data.Count() > 0)
                {
                    throw new Exception("NAME 不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    conn.Open();
                    using (IDbTransaction transaction = conn.BeginTransaction())
                    {
                        var sql = @" INSERT INTO [Demo]
		                                ([NAME])
	                                 VALUES
		                                (@NAME)";
                        conn.Execute(sql, model);
                        transaction.Commit();
                        return Json("新增成功");
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        // PUT api/Demo/5
        public IHttpActionResult Put([FromBody]DemoModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model?.ID))
                {
                    throw new Exception("ID 不能為空");
                }
                if (string.IsNullOrEmpty(model?.NAME))
                {
                    throw new Exception("NAME 不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    var sql = @" UPDATE [Demo]
	                            SET [NAME] = @NAME
	                            WHERE ID=@ID";
                    conn.Execute(sql, model);

                    return Json("修改成功");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// 刪除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // DELETE api/Demo/5
        public IHttpActionResult Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    throw new Exception("ID 不能為空");
                }
                using (var conn = new System.Data.SQLite.SQLiteConnection(_connectionStr))
                {
                    var sql = @" DELETE FROM [Demo]
		                        WHERE ID=@ID ";
                    conn.Execute(sql, new { ID = id });
                    return Json("刪除成功");
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
