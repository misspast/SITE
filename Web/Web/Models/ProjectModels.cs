﻿using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class Project : IListPage
    {
        [Display(Name = "唯一编号")]
        public Guid Id { get; set; }

        [Display(Name = "管理员")]
        public virtual User Admin { get; set; }

        [Display(Name = "项目图标")]
        public virtual Material Avatar { get; set; }

        [Required]
        [Display(Name = "项目名称")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "目标行业")]
        public string Industry { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "项目介绍")]
        public string Introduction { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "预计产品")]
        public string Product { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "产品特点")]
        public string Feature { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "技术特点")]
        public string Tech { get; set; }

        [Required]
        [Display(Name = "项目进程")]
        public ProjectProgressType Progress { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "专利所有")]
        public string Patent { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "希望获得的支持")]
        public string Desire { get; set; }

        [Required]
        [DataType(DataType.MultilineText)]
        [Display(Name = "目标人群")]
        public string TargetCustomer { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Display(Name = "项目预算")]
        public string ProjectBudget { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "项目网页")]
        public string Webpage { get; set; }

        [Required]
        [Display(Name = "公开项目信息")]
        public bool Privacy { get; set; }

        [Display(Name = "管理员批复")]
        public string Note { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        [Display(Name = "状态")]
        public ProjectStatus Status { get; set; }

        [Display(Name = "团队")]
        public virtual Team Team { get; set; }

        public void NewProject(BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Admin = db.Users.Find(Extensions.GetCurrentUser().Id);
            Time = DateTime.Now;
            Status = ProjectStatus.ToApprove;
        }
    }

    public class Team : IListPage
    {
        [Display(Name = "唯一编号")]
        public Guid Id { get; set; }

        [Display(Name = "团队名称")]
        public string Name { get; set; }

        [Display(Name = "管理员")]
        public virtual User Admin { get; set; }

        [Display(Name = "创建时间")]
        public DateTime Time { get; set; }

        [Display(Name = "团队介绍")]
        public string Introduction { get; set; }

        [Display(Name = "团队公告")]
        public string Announcement { get; set; }

        [Display(Name = "公开招募")]
        public bool Searchable { get; set; }

        [Display(Name = "注册公司")]
        public virtual Company Company { get; set; }

        [Display(Name = "团队成员")]
        public virtual List<TeamRecord> Member { get; set; }

        [Display(Name = "团队记录")]
        public virtual List<TeamEvent> Events { get; set; }

        [Display(Name = "团队报告")]
        public virtual List<TeamReport> Reports { get; set; }

        [Display(Name = "报告状态")]
        public bool ReportUpdated { get; set; }

        public void NewTeam(ref Project project)
        {
            Id = Guid.NewGuid();
            Name = project.Name;
            Admin = project.Admin;
            Time = DateTime.Now;
            Introduction = "此处的信息将作为团队的对外介绍。";
            Announcement = "此处的信息将作为团队的内部公告";
            Searchable = true;
            ReportUpdated = false;
            project.Team = this;
            var db = new BaseDbContext();
            var TeamRecord = new TeamRecord(this, TeamMemberStatus.Admin, Extensions.GetContextUser(ref db));
        }
    }

    public class TeamRecord : Remark
    {
        [Display(Name = "团队")]
        public virtual Team Team { get; set; }

        [Display(Name = "状态")]
        public TeamMemberStatus Status { get; set; }

        public TeamRecord() : base() { }

        public TeamRecord(Team team) : base()
        {
            Team = team;
            Status = TeamMemberStatus.Normal;
            Time = DateTime.Parse("2000-1-1");
        }

        public TeamRecord(Team team, TeamMemberStatus status) : base()
        {
            Team = team;
            Status = status;
            Time = DateTime.Parse("2000-1-1");
        }

        public TeamRecord(Team team, TeamMemberStatus status, User user) : base()
        {
            Team = team;
            Status = status;
            Receiver = user;
            Time = DateTime.Parse("2000-1-1");
        }
    }

    public class Company : IListPage
    {
        [Display(Name = "公司申请编号")]
        public Guid Id { get; set; }

        [Display(Name = "公司创始人")]
        public virtual User Admin { get; set; }

        [Display(Name = "目标行业")]
        public string Industry { get; set; }

        [Display(Name = "商业计划书")]
        public virtual Material Plan { get; set; }

        [Display(Name = "管理员批复")]
        public string Note { get; set; }

        [Display(Name = "申请时间")]
        public DateTime Time { get; set; }

        [Display(Name = "申请状态")]
        public CompanyStatus Status { get; set; }

        public void NewCompany(ref BaseDbContext db)
        {
            Id = Guid.NewGuid();
            Admin = db.Users.Find(Extensions.GetCurrentUser().Id);
            Time = DateTime.Now;
            Status = CompanyStatus.ToApprove;
        }
    }

    public enum ProjectStatus
    {
        [EnumDisplayName("无记录")]
        None,
        [EnumDisplayName("未通过")]
        Denied,
        [EnumDisplayName("待审批")]
        ToApprove,
        [EnumDisplayName("已通过")]
        Done
    }

    public enum CompanyStatus
    {
        [EnumDisplayName("无记录")]
        None,
        [EnumDisplayName("未通过")]
        Denied,
        [EnumDisplayName("待审批")]
        ToApprove,
        [EnumDisplayName("已通过")]
        Done
    }

    public enum TeamMemberStatus
    {
        [EnumDisplayName("成员")]
        Normal,
        [EnumDisplayName("管理员")]
        Admin,
        [EnumDisplayName("申请者")]
        Apply,
        [EnumDisplayName("招募者")]
        Recruit
    }

    public enum ProjectProgressType
    {
        [EnumDisplayName("项目尚未开始进行")]
        None,
        [EnumDisplayName("已经组建团队，尚在创业前期")]
        HaveTeam,
        [EnumDisplayName("创业进入中期阶段，预期稳定")]
        Done,
        [EnumDisplayName("创业后期，已经注册公司")]
        HaveCompany
    }

    public class IndustryList
    {
        public Guid ID { get; set; }

        public string IndustryName { get; set; }
    }
}