using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Web;

namespace XkSystem.Code
{
    public class EnumHelper
    {
        /// <summary>
        /// 上下学交通工具
        /// </summary>
        public enum SchoolTransportationType
        {
            /// <summary>
            /// 家长接送
            /// </summary>
            [Display(Name = "家长接送")]
            ParentsPick = 1,

            /// <summary>
            /// 坐公交车
            /// </summary>
            [Display(Name = "坐公交车")]
            ByBus = 2,

            /// <summary>
            /// 骑自行车
            /// </summary>
            [Display(Name = "骑自行车")]
            ByBike = 3,

            /// <summary>
            /// 乘坐电动车或摩托车
            /// </summary>
            [Display(Name = "乘坐电动车或摩托车")]
            ByMotorcycle = 4,

            /// <summary>
            /// 乘坐校车
            /// </summary>
            [Display(Name = "乘坐校车")]
            BySchoolBus = 5,

            /// <summary>
            /// 步行
            /// </summary>
            [Display(Name = "步行")]
            ByWalk = 6
        }

        public enum CheckStatus
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Display(Name = "未处理")]
            None = 0,

            /// <summary>
            /// 通过
            /// </summary>
            [Display(Name = "通过")]
            Success = 1,

            /// <summary>
            /// 不通过
            /// </summary>
            [Display(Name = "不通过")]
            Fail = -1
        }

        public enum ConfigType
        {
            /// <summary>
            /// 启动页
            /// </summary>
            Startup = 1,

            /// <summary>
            /// 登录提示
            /// </summary>
            LoginMessage = 2,

            /// <summary>
            /// 版权信息
            /// </summary>
            Copyright = 3,

            /// <summary>.
            /// 
            /// 默认Title
            /// </summary>
            Title = 4,

            /// <summary>
            /// 是否英文
            /// </summary>
            IsEnglish = 5,

            /// <summary>
            /// 注册码
            /// </summary>
            CdKey = 6,
        }

        public enum ProgramCode
        {
            /// <summary>
            /// 教务系统
            /// </summary>
            EAS = 2,

            /// <summary>
            /// 监考抽签
            /// </summary>
            Draw = 3,

            /// <summary>
            /// 工资系统
            /// </summary>
            Pay = 4,

            /// <summary>
            /// 资源库
            /// </summary>
            Ware = 5,

            /// <summary>
            /// 站群
            /// </summary>
            Portal = 6,

            /// <summary>
            /// 招聘
            /// </summary>
            Job = 7,

            /// <summary>
            /// 学生综合素养
            /// </summary>
            Quality = 8,

            /// <summary>
            /// 资产管理
            /// </summary>
            Asset = 9,

            /// <summary>
            /// 德育系统
            /// </summary>
            Moral = 10,

            /// <summary>
            /// 教师绩效
            /// </summary>
            Kpi = 11,

            /// <summary>
            /// 办公系统
            /// </summary>
            OA = 12,

            /// <summary>
            /// 云桌面
            /// </summary>
            Desktop = 13,

            /// <summary>
            /// 基础数据库
            /// </summary>
            Base = 14,

            /// <summary>
            /// 招生管理
            /// </summary>
            Admit = 15,

            /// <summary>
            /// 排课系统
            /// </summary>
            Timetable = 16,

            /// <summary>
            /// 成绩分析
            /// </summary>
            ExamAnalyze = 17,

            /// <summary>
            /// 公开课
            /// </summary>
            Open = 18,

            /// <summary>
            /// 移动应用
            /// </summary>
            App = 19,

            /// <summary>
            /// 教师绩效2-北师大附小
            /// </summary>
            Kpi2 = 20
        }

        public enum AssetApply
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Display(Name = "未处理")]
            None = 0,

            /// <summary>
            /// 通过
            /// </summary>
            [Display(Name = "通过")]
            Success = 1,

            /// <summary>
            /// 不通过
            /// </summary>
            [Display(Name = "不通过")]
            Fail = -1,

            /// <summary>
            /// 已领用
            /// </summary>
            [Display(Name = "已领用")]
            Receive = 2
        }

        public enum AssetRepair
        {
            /// <summary>
            /// 未处理
            /// </summary>
            [Display(Name = "未处理")]
            UnProcessed = 1,


            /// <summary>
            /// 处理中
            /// </summary>
            [Display(Name = "处理中")]
            Processing = 2,


            /// <summary>
            /// 已处理
            /// </summary>
            [Display(Name = "已处理")]
            Processed = 3
        }

        /// <summary>
        /// 资产状态
        /// </summary>
        public enum AssetStatus
        {
            /// <summary>
            /// 正常
            /// </summary>
            [Display(Name = "正常")]
            Normal = 1,

            /// <summary>
            /// 维修中
            /// </summary>
            [Display(Name = "维修中")]
            Repair = 2,

            /// <summary>
            /// 报废
            /// </summary>
            [Display(Name = "报废")]
            Scrap = 3,

            /// <summary>
            /// 借用中
            /// </summary>
            [Display(Name = "借用中")]
            InService = 4,

            /// <summary>
            /// 丢失
            /// </summary>
            [Display(Name = "丢失")]
            Lost = 5,

            /// <summary>
            /// 损坏
            /// </summary>
            [Display(Name = "损坏")]
            Failure = 6


        }

        /// <summary>
        /// 资产类型
        /// </summary>
        public enum AssetType
        {

            /// <summary>
            /// 固定资产
            /// </summary>
            [Display(Name = "固定资产")]
            FixedAssets = 1,


            /// <summary>
            /// 消耗品
            /// </summary>
            [Display(Name = "消耗品")]
            Consumables = 2

        }

        public enum ClassAllotType
        {
            // 分班方式（按顺序，按S曲线，顺序为按班1、2、3、1、2、3的方式，按S曲线为1、2、3、3、2、1的方式保证学生质量均衡）
            // 注意分班时需要按性别再按分班成绩进行排序，最后一个推送到到正式分班中

            /// <summary>
            /// 按顺序：顺序为按班1、2、3、1、2、3的方式
            /// </summary>
            [Display(Name = "按顺序")]
            ByOrder = 1,

            /// <summary>
            /// 按S曲线:按S曲线为1、2、3、3、2、1的方式保证学生质量均衡
            /// </summary>
            [Display(Name = "按S曲线")]
            ByCurveS = 2
        }

        public enum CourseScheduleType
        {
            /// <summary>
            /// 默认
            /// </summary>
            [Display(Name = "默认")]
            All = 0,

            /// <summary>
            /// 单周
            /// </summary>
            [Display(Name = "单周")]
            Odd = 1,

            /// <summary>
            /// 双周
            /// </summary>
            [Display(Name = "双周")]
            Dual = 2
        }

        /// <summary>
        /// 网盘授权
        /// </summary>
        public enum DiskPermit
        {
            /// <summary>
            /// 私有
            /// </summary>
            [Display(Name = "私有")]
            Private = 0,

            /// <summary>
            /// 共有
            /// </summary>
            [Display(Name = "共有")]
            Public = 1,

            /// <summary>
            /// 需授权
            /// </summary>
            [Display(Name = "需授权")]
            Authorize = 2
        }

        /// <summary>
        /// 网盘类型
        /// </summary>
        public enum DiskType
        {
            /// <summary>
            /// 私有
            /// </summary>
            [Display(Name = "私有")]
            Private = 0,

            /// <summary>
            /// 公共
            /// </summary>
            [Display(Name = "公共")]
            Public = 1
        }

        public enum DrawRule
        {
            /// <summary>
            /// 随机
            /// </summary>
            None = 0,

            /// <summary>
            /// 本校
            /// </summary>
            Same = 1,

            /// <summary>
            /// 外校
            /// </summary>
            Different = 2
        }

        public enum ElectiveRule
        {
            /// <summary>
            /// 前置
            /// </summary>
            [Display(Name = "前置")]
            Front = 1,

            /// <summary>
            /// 互斥
            /// </summary>
            [Display(Name = "互斥")]
            Exclude = 2,

            /// <summary>
            /// 后置
            /// </summary>
            [Display(Name = "后置")]
            Behind = 3
        }

        public enum ElectiveType
        {
            /// <summary>
            /// 列表选课
            /// </summary>
            List = 1,

            /// <summary>
            /// 课表选课
            /// </summary>
            WeekPeriod = 2
        }

        public enum ExamMarkTag
        {
            /// <summary>
            /// 未标记
            /// </summary>
            [Display(Name = "")]
            None = 0,

            /// <summary>
            /// 缺考
            /// </summary>
            [Display(Name = "缺考")]
            Absent = 1,

            /// <summary>
            /// 作弊
            /// </summary>
            [Display(Name = "作弊")]
            Cheat = 2
        }

        public enum FormFieldDefined
        {
            [Display(Name = "未定义")]
            Default = 0,

            [Display(Name = "流程发起人")]
            CreateUser = 1,

            [Display(Name = "流程发起时间")]
            CreateTime = 2,

            [Display(Name = "发起人所在部门")]
            CreateDept = 3,

            [Display(Name = "审批人")]
            CheckUser = 4,

            [Display(Name = "审批时间")]
            CheckTime = 5,

            [Display(Name = "审批人部门")]
            CheckDept = 6
        }

        public enum FormFieldType
        {
            /// <summary>
            /// 文本
            /// </summary>
            [Display(Name = "文本")]
            Text = 0,

            /// <summary>
            /// 整数
            /// </summary>
            [Display(Name = "整数")]
            Int = 1,

            /// <summary>
            /// 小数
            /// </summary>
            [Display(Name = "小数")]
            Decimal = 2,

            /// <summary>
            /// 下拉框
            /// </summary>
            [Display(Name = "下拉框")]
            Dropdownlist = 3,

            /// <summary>
            /// 复选框
            /// </summary>
            [Display(Name = "复选框")]
            CheckBox = 4,

            /// <summary>
            /// 单选框
            /// </summary>
            [Display(Name = "单选框")]
            Radio = 5,

            /// <summary>
            /// 图片
            /// </summary>
            [Display(Name = "图片")]
            Image = 6,

            /// <summary>
            /// 附件
            /// </summary>
            [Display(Name = "附件")]
            File = 7,

            /// <summary>
            /// 日期
            /// </summary>
            [Display(Name = "日期")]
            Date = 8,

            /// <summary>
            /// 自定义
            /// </summary>
            [Display(Name = "自定义")]
            Defined = 99,
        }

        /// <summary>
        /// 排列方式
        /// 示例：复选框组
        /// </summary>
        public enum FormFieldRepeatDirection
        {
            /// <summary>
            /// 横排
            /// </summary>
            [Display(Name = "横排")]
            Horizontal = 0,

            /// <summary>
            /// 竖排
            /// </summary>
            [Display(Name = "竖排")]
            Vertical = 1
        }

        /// <summary>
        /// 对齐方式
        /// </summary>
        public enum FormFieldTextAlign
        {
            /// <summary>
            /// 左对齐
            /// </summary>
            [Display(Name = "左对齐")]
            Left = 0,

            /// <summary>
            /// 居中对齐
            /// </summary>
            [Display(Name = "居中对齐")]
            Center = 1,

            /// <summary>
            /// 右对齐
            /// </summary>
            [Display(Name = "右对齐")]
            Right = 2
        }

        /// <summary>
        /// 流程模式
        /// </summary>
        public enum FlowMode
        {
            /// <summary>
            /// 固定流程
            /// </summary>
            [Display(Name = "固定流程")]
            Fixed = 1,

            /// <summary>
            /// 自由流程
            /// </summary>
            [Display(Name = "自由流程")]
            Free = 2
        }

        /// <summary>
        /// 流程权限
        /// </summary>
        public enum FlowPower
        {
            /// <summary>
            /// 发起
            /// </summary>
            [Display(Name = "发起")]
            Apply = 1,

            /// <summary>
            /// 管理
            /// </summary>
            [Display(Name = "管理")]
            Admin = 9
        }

        /// <summary>
        /// 流程连线过滤条件
        /// </summary>
        public enum FlowLineFilter
        {
            /// <summary>
            /// 满足任意条件
            /// </summary>
            [Display(Name = "满足任意条件")]
            Any = 1,

            /// <summary>
            /// 满足所有条件
            /// </summary>
            [Display(Name = "满足所有条件")]
            All = 2
        }

        /// <summary>
        /// 流程规则操作符
        /// </summary>
        public enum FlowRuleOperator
        {
            /// <summary>
            /// =
            /// </summary>
            [Display(Name = "=")]
            Equal = 1,

            /// <summary>
            /// ≠
            /// </summary>
            [Display(Name = "≠")]
            NotEqual = 2
        }

        /// <summary>
        /// 德育选项表达式
        /// </summary>
        public enum MoralExpress
        {
            /// <summary>
            /// 加
            /// </summary>
            [Display(Name = "加")]
            Add = 0,

            /// <summary>
            /// 减
            /// </summary>
            [Display(Name = "减")]
            Subtract = 2,
        }

        /// <summary>
        /// 德育选项评价对象
        /// </summary>
        public enum MoralItemKind
        {
            [Display(Name = "学生")]
            Student = 0,

            [Display(Name = "小组")]
            Group = 1,

            [Display(Name = "班级")]
            Class = 2,
        }

        /// <summary>
        /// 德育操作模式
        /// </summary>
        public enum MoralItemOperateType
        {

            [Display(Name = "打分")]
            Score = 0,

            [Display(Name = "评语")]
            Comment
        }

        /// <summary>
        /// 德育选项类型
        /// </summary>
        public enum MoralItemType
        {
            /// <summary>
            /// 下拉框
            /// </summary>
            [Display(Name = "下拉框")]
            Select = 0,

            /// <summary>
            /// 输入框
            /// </summary>
            [Display(Name = "输入框")]
            Text = 1
        }

        /// <summary>
        /// 德育评价方式(多次、一次、每天)
        /// </summary>
        public enum MoralType
        {
            /// <summary>
            /// 多次
            /// </summary>
            [Display(Name = "多次")]
            Many = 0,

            /// <summary>
            /// 一次
            /// </summary>
            [Display(Name = "一次")]
            Once = 1,

            /// <summary>
            /// 每天
            /// </summary>
            [Display(Name = "每天")]
            Days = 2
        }

        public enum PayAccountType
        {
            /// <summary>
            /// 邮政编码
            /// </summary>
            UserCode,
            /// <summary>
            /// 用户名
            /// </summary>
            UserName
        }

        /// <summary>
        /// 输入框类型
        /// </summary>
        public enum PayInputType
        {
            /// <summary>
            /// 文本框
            /// </summary>
            TextBox,

            /// <summary>
            /// 复选框
            /// </summary>
            CheckBox,

            /// <summary>
            /// 单选框
            /// </summary>
            Radio,

            /// <summary>
            /// 多行文本
            /// </summary>
            MuliText,

            /// <summary>
            /// 下拉框
            /// </summary>
            Dropdownlist,

            /// <summary>
            /// 文本框
            /// </summary>
            Label,

            /// <summary>
            /// 文本框
            /// </summary>
            Total
        }

        public enum PayItemDefined
        {
            /// <summary>
            /// 未定义
            /// </summary>
            [Display(Name = "")]
            Null = 0,

            /// <summary>
            /// 人员信息
            /// </summary>
            [Display(Name = "人员编号")]
            TeacherCode = 1,

            /// <summary>
            /// 人员姓名
            /// </summary>
            [Display(Name = "人员姓名")]
            TeacherName = 3,

            /// <summary>
            /// 开户银行
            /// </summary>
            [Display(Name = "开户银行")]
            MemberBankName = 5,

            /// <summary>
            /// 银行卡号
            /// </summary>
            [Display(Name = "银行卡号")]
            MemberBankNumber = 7,

            /// <summary>
            /// 身份证号
            /// </summary>
            [Display(Name = "身份证号")]
            MemberIdentityNumber = 9,

            /// <summary>
            /// 人事分组
            /// </summary>
            [Display(Name = "人事分组")]
            MemberGroup = 11,

            /// <summary>
            /// 人事岗位
            /// </summary>
            [Display(Name = "人事岗位")]
            MemberJob = 13,

            /// <summary>
            /// 人事类型
            /// </summary>
            [Display(Name = "人事类型")]
            MemberType = 15,

            /// <summary>
            /// 人事部门
            /// </summary>
            [Display(Name = "人事部门")]
            PayDept = 17,

            /// <summary>
            /// 岗位级别
            /// </summary>
            [Display(Name = "岗位级别")]
            MemberLevel = 19
        }

        /// <summary>
        /// 工资项类型
        /// </summary>
        public enum PayItemType
        {
            /// <summary>
            /// 数值
            /// </summary>
            [Display(Name = "数值")]
            Number = 0,

            /// <summary>
            /// 文本
            /// </summary>
            [Display(Name = "文本")]
            Text = 1,

            /// <summary>
            /// 公式
            /// </summary>
            [Display(Name = "公式")]
            Rule = 3,

            /// <summary>
            /// 勾选框
            /// </summary>
            [Display(Name = "勾选框")]
            Checkbox = 5,

            /// <summary>
            /// 下拉框
            /// </summary>
            [Display(Name = "下拉框")]
            Dropdownlist = 7,

            /// <summary>
            /// 汇总(分类)
            /// </summary>
            [Display(Name = "汇总(分类)")]
            Total = 8,

            /// <summary>
            /// 自定义
            /// </summary>
            [Display(Name = "自定义")]
            Defined = 9,
        }

        public enum PayPower
        {
            /// <summary>
            /// 录取权限
            /// </summary>
            [Display(Name = "录取权限")]
            Input = 1,

            /// <summary>
            /// 审批权限
            /// </summary>
            [Display(Name = "审批权限")]
            Check = 2,

            /// <summary>
            /// 报表权限
            /// </summary>
            [Display(Name = "报表权限")]
            Report = 3,

            /// <summary>
            /// 管理权限
            /// </summary>
            [Display(Name = "管理权限")]
            Admin = 9
        }

        public enum QualityItemType
        {
            /// <summary>
            /// 单选题
            /// </summary>
            [Display(Name = "单选题")]
            Radio = 0,

            /// <summary>
            /// 多选题
            /// </summary>
            [Display(Name = "多选题")]
            CheckBox = 1,

            /// <summary>
            /// 问答题
            /// </summary>
            [Display(Name = "问答题")]
            TextBox = 2
        }

        public enum QuestionnaireItemType
        {
            /// <summary>
            /// 单选题
            /// </summary>
            [Display(Name = "单选题")]
            Radio = 0,

            /// <summary>
            /// 多选题
            /// </summary>
            [Display(Name = "多选题")]
            CheckBox = 1,

            /// <summary>
            /// 问答题
            /// </summary>
            [Display(Name = "问答题")]
            TextBox = 2
        }

        public enum ReserveKind
        {
            /// <summary>
            /// 按节次
            /// </summary>
            [Display(Name = "按节次")]
            Period = 1,

            /// <summary>
            /// 按时间
            /// </summary>
            [Display(Name = "按时间")]
            DateTime = 2
        }

        public enum SmsSendStatus
        {
            /// <summary>
            /// 未定义
            /// </summary>
            [Display(Name = "未发送")]
            NoSend = 0,

            /// <summary>
            /// 人员信息
            /// </summary>
            [Display(Name = "发送成功")]
            Fail = 1,

            /// <summary>
            /// 人员姓名
            /// </summary>
            [Display(Name = "失败")]
            Success = -1,
        }

        public enum SmsServerType
        {
            [Display(Name = "未知")]
            None = 0,

            [Display(Name = "开唯教育")]
            KaiWei = 1,

            [Display(Name = "阿里大鱼")]
            Aali = 2
        }

        public enum StudentHonorSource
        {
            /// <summary>
            /// 初始录入
            /// </summary>
            [Display(Name = "初始录入")]
            Init = 1,

            /// <summary>
            /// 教师录入
            /// </summary>
            [Display(Name = "教师录入")]
            Teacher = 2,

            /// <summary>
            /// 申请
            /// </summary>
            [Display(Name = "申请")]
            Apply = 3
        }

        public enum SurveyItemType
        {
            /// <summary>
            /// 单选题
            /// </summary>
            [Display(Name = "单选题")]
            Radio = 0,

            /// <summary>
            /// 多选题
            /// </summary>
            [Display(Name = "多选题")]
            CheckBox = 1,

            /// <summary>
            /// 问答题
            /// </summary>
            [Display(Name = "问答题")]
            TextBox = 2
        }

        public enum SurveyType
        {
            /// <summary>
            /// 任课教师
            /// </summary>
            [Display(Name = "任课教师")]
            Org = 0,

            /// <summary>
            /// 班主任
            /// </summary>
            [Display(Name = "班主任")]
            Class = 1,

            /// <summary>
            /// 科目调查
            /// </summary>
            [Display(Name = "科目调查")]
            Vote = 2
        }

        public enum SysFunctionMethod
        {
            /// <summary>
            /// Get方法
            /// </summary>
            Get = 0,

            /// <summary>
            /// Post方法
            /// </summary>
            Post = 1
        }

        public enum SysMenuCode
        {
            /// <summary>
            /// 无规则
            /// </summary>
            Other = 0,

            /// <summary>
            /// 工资录入
            /// </summary>
            PayInput = 71,

            /// <summary>
            /// 工资审批
            /// </summary>
            PayCheck = 72,
        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public enum SysRoleCode
        {
            /// <summary>
            /// 自定义
            /// </summary>
            Other = 0,

            /// <summary>
            /// 管理员
            /// </summary>
            Administrator = 1,

            /// <summary>
            /// 教师
            /// </summary>
            Teacher = 10,

            /// <summary>
            /// 年级组长
            /// </summary>
            GradeTeacher = 11,

            /// <summary>
            /// 科组长
            /// </summary>
            SubjectTeacher = 12,

            /// <summary>
            /// 班主任
            /// </summary>
            ClassTeacher = 13,

            /// <summary>
            /// 任课教师
            /// </summary>
            OrgTeacher = 14,

            /// <summary>
            /// 学生
            /// </summary>
            Student = 50,

            /// <summary>
            /// 家长
            /// </summary>
            Family = 51,

            /// <summary>
            /// 报修受理人
            /// </summary>
            Repair = 52,

            /// <summary>
            /// 报修管理人
            /// </summary>
            RepairManagner = 53,

            ///// <summary>
            ///// 德育管理员
            ///// </summary>
            //MoralManager = 54
        }

        /// <summary>
        /// 技术支持类型
        /// </summary>
        public enum SysSupportType
        {
            /// <summary>
            /// 电话
            /// </summary>
            [Display(Name = "电话")]
            Phone = 1,

            /// <summary>
            /// QQ
            /// </summary>
            [Display(Name = "QQ")]
            QQ = 1,

            /// <summary>
            /// 邮件
            /// </summary>
            [Display(Name = "邮件")]
            Email = 2
        }

        public enum SysConfig
        {

        }

        /// <summary>
        /// 用户类型
        /// </summary>
        public enum SysUserType
        {
            /// <summary>
            /// 其他
            /// </summary>
            [Display(Name = "其他")]
            Other = 0,

            /// <summary>
            /// 教师
            /// </summary>
            [Display(Name = "教师")]
            Teacher = 1,

            /// <summary>
            /// 学生
            /// </summary>
            [Display(Name = "学生")]
            Student = 2,

            /// <summary>
            /// 家长
            /// </summary>
            [Display(Name = "家长")]
            Family = 3,

            /// <summary>
            /// 系统管理员
            /// </summary>
            [Display(Name = "管理员")]
            Administrator = 9,
        }

        public enum TestItemLayout
        {
            /// <summary>
            /// 横向
            /// </summary>
            [Display(Name = "横向")]
            Horizontal = 0,

            /// <summary>
            /// 纵向
            /// </summary>
            [Display(Name = "纵向")]
            Vertical = 1
        }

        public enum TestItemType
        {
            /// <summary>
            /// 单选
            /// </summary>
            [Display(Name = "单选")]
            Radio = 0,

            /// <summary>
            /// 多选
            /// </summary>
            [Display(Name = "多选")]
            Select = 1,

            /// <summary>
            /// 问答
            /// </summary>
            [Display(Name = "问答")]
            Text = 2
        }

        public enum VoteItemType
        {
            /// <summary>
            /// 单选题
            /// </summary>
            [Display(Name = "单选题")]
            Radio = 0,

            /// <summary>
            /// 多选题
            /// </summary>
            [Display(Name = "多选题")]
            CheckBox = 1,

            /// <summary>
            /// 问答题
            /// </summary>
            [Display(Name = "问答题")]
            TextBox = 2
        }

        public enum YearType
        {
            /// <summary>
            /// 学年
            /// </summary>
            [Display(Name = "学年")]
            Year = 0,

            /// <summary>
            /// 学期
            /// </summary>
            [Display(Name = "学期")]
            Term = 1,

            /// <summary>
            /// 学段
            /// </summary>
            [Display(Name = "学段")]
            Section = 2
        }

        public enum ReserveStatus
        {
            /// <summary>
            /// 立即预约
            /// </summary>
            [Display(Name = "立即预约")]
            StartReserve = 0,

            /// <summary>
            /// 停止预约
            /// </summary>
            [Display(Name = "停止预约")]
            StopReserve = 1,

            /// <summary>
            /// 取消预约
            /// </summary>
            [Display(Name = "取消预约")]
            CancelReserve = 2
        }

        /// <summary>
        /// OA流程节点审批状态
        /// </summary>
        public enum OAFlowNodeStatus
        {
            /// <summary>
            /// 未审批
            /// </summary>
            [Display(Name = "未审批")]
            NotReviewed = 0,

            /// <summary>
            /// 已审批
            /// </summary>
            [Display(Name = "已审批")]
            Approved = 1,

            /// <summary>
            /// 已驳回
            /// </summary>
            [Display(Name = "已驳回")]
            WithoutApproval = -1
        }

        public enum OpenClassType
        {
            /// <summary>
            /// 普通课（不需审批）
            /// </summary>
            [Display(Name = "普通课")]
            NormalClass = 0,

            /// <summary>
            /// 公开课（需要审批）
            /// </summary>
            [Display(Name = "公开课")]
            OpenClass = 1
        }

        public enum OpenSurveyItemType
        {
            /// <summary>
            /// 单选题
            /// </summary>
            [Display(Name = "单选题")]
            Radio = 0,

            /// <summary>
            /// 多选题
            /// </summary>
            [Display(Name = "多选题")]
            CheckBox = 1,

            /// <summary>
            /// 问答题
            /// </summary>
            [Display(Name = "问答题")]
            TextBox = 2
        }


        public enum SmsTempletType
        {
            [Display(Name = "考勤")]
            Attencance = 0
        }

        public enum StudentChangeType
        {
            [Display(Name = "离校")]
            OutSchool = 0,

            [Display(Name = "在校")]
            InSchool = 1
        }
    }
}

public static class EnumExtend
{
    /// <summary>
    /// 从枚举中获取Description @(BindData.GetDescription((AwardType)item.AwardType))
    /// </summary>
    /// <param name="enumName"></param>
    /// <returns></returns>
    public static string GetDescription(this Enum enumName)
    {
        string _description = string.Empty;
        var _fieldInfo = enumName.GetType().GetField(enumName.ToString());
        var _attributes = _fieldInfo.GetDescriptAttr();
        if (_attributes != null && _attributes.Length > 0)
        {
            _description = _attributes[0].GetName();
        }
        else
        {
            _description = enumName.ToString();
            //_description = "未定义";
        }
        return _description;
    }

    public static int GetOrder(this Enum enumName)
    {
        int? order = 0;
        var _fieldInfo = enumName.GetType().GetField(enumName.ToString());
        var _attributes = _fieldInfo.GetDescriptAttr();
        if (_attributes != null && _attributes.Length > 0)
        {
            order = _attributes[0].GetOrder();
        }

        if (order != null)
        {
            return (int)order;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// 根据Description获取枚举
    /// </summary>
    /// <param name="description"></param>
    /// <returns></returns>
    public static T GetEnumName<T>(string description)
    {
        Type _type = typeof(T);
        foreach (var field in _type.GetFields())
        {
            var _curDesc = field.GetDescriptAttr();
            if (_curDesc != null && _curDesc.Length > 0)
            {
                if (_curDesc[0].GetName() == description)
                    return (T)field.GetValue(null);
            }
            else
            {
                if (field.Name == description)
                    return (T)field.GetValue(null);
            }
        }
        throw new ArgumentException(string.Format("{0} 未能找到对应的枚举.", description), "Description");
    }

    public static DisplayAttribute[] GetDescriptAttr(this FieldInfo fieldInfo)
    {
        if (fieldInfo != null)
        {
            //return (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return (DisplayAttribute[])fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false);
        }

        return null;
    }
}
