﻿using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private BaseDbContext db = new BaseDbContext();

        public ActionResult Index(string select)
        {
            return View(CourseOperation.List(select, false));
        }
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseOperation courseOperation = db.CourseOperations.Find(id);
            if (courseOperation == null)
            {
                return HttpNotFound();
            }
            return View(courseOperation);
        }

        public ActionResult Apply(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var CourseOperation = db.CourseOperations.Find(Id);
                if (CourseOperation == null)
                {
                    return new HttpStatusCodeResult(404);
                }
                if (CourseOperation.Students != null)
                {
                    if (CourseOperation.Students.Contains(db.Users.Find(HttpContext.User.Identity.GetUserId())))
                    {
                        TempData["Alert"] = "您已选过该课程！";
                        return RedirectToAction("Index");
                    }
                }
                if (CourseOperation.Count >= CourseOperation.Limit)
                {
                    TempData["Alert"] = "该课程已满！";
                    return RedirectToAction("Index");
                }
                if (DateTime.Now > CourseOperation.StartTime)
                {
                    TempData["Alert"] = "该课程现在不可预约！";
                    return RedirectToAction("Index");
                }
                var courseRecord = new CourseRecord();
                if (courseRecord.Apply(CourseOperation.Id))
                {
                    TempData["Alert"] = "选课成功！";
                    return RedirectToAction("Index");
                }
                TempData["Alert"] = "你不符合选课要求";
            }
            return RedirectToAction("Index");
        }
        public ActionResult Quit(Guid Id)
        {
            if (ModelState.IsValid)
            {
                var courseOperation = db.CourseOperations.Find(Id);
                var user = Extensions.GetContextUser(ref db);
                if (courseOperation == null)
                    return new HttpStatusCodeResult(404);
                else
                {
                    if (DateTime.Now > courseOperation.StartTime)
                    {
                        TempData["Alert"] = "现在不是可退选的时间！";
                    }
                    if (courseOperation.Students != null)
                    {
                        if (!courseOperation.Students.Contains(user))
                        {
                            TempData["Alert"] = "您未选过该课程！";
                        }
                        else
                        {
                            courseOperation.Students.Remove(user);
                            db.CourseRecords.Remove(db.CourseRecords.Where(c => c.CourseOperation.Id == courseOperation.Id && c.Receiver.Id == user.Id).First());
                            courseOperation.Count--;
                            db.SaveChanges();
                            if (courseOperation.Students.Contains(user))
                                TempData["Alert"] = "退课失败";
                            else
                                TempData["Alert"] = "退课成功";
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
