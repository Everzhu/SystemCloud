
; var FormUI = {
    regint: /^(-?[1-9]\d*|0)$/                   //整数
    , regzint: /^[1-9]\d*$/                      //正整数
    , regfint: /^-[1-9]\d*$/                     //负整数
    , regfloat: /^(-?\d+)(\.\d+)?$/              //小数
    , regpercent: /^(([1-9]\d*)|(\d+(\.)\d+))%$/ //百分数
    , msgfloat: "数据必须为小数（如：12.34）！"
    , msgpercent: "数据必须为百分数（如：8%,须加%结尾）！"
    , curRootPath: ''
    , BaseData: null
    , PageType: null         //0 预览 1 其他
    , count1: 0
    , count2: 0
    , isHistory: 0
    , isSaveSuccess: true
    , isSubmit: false
    , isSaveing: false
    , listenResult: null
    , strIncreaseValid: '' //自增表格控件验证jsm
    , strRealValid: ''     //实时验证js
    , strSubmitValid: ''   //提交时验证js
    , showArray: null      //流程按钮显示
    , FlowIsOpen: 0        //用户是否有打开表单权限 0 无  1 流程中有用户  2 用户可管理流程
    , FlowIsDraft: 0       //草稿 0 否 1 是
    , FormVID: ''          //表单版本编号
    , FlowVID: ''          //流程版本编号
    , FlowApplyID: 0       //流程编号
    , FlowApplyStepID: 0   //流程步骤编号
    , FlowPage: 0          //0查看 1发送  2待办
    , FlowStatus: 0        //-1预览 0查看 1发送  2待办
    , FlowModel: 0         //0 未知 1固定流程 2 自由流程
    , FlowNodeID: ''       //流程节点编号    
    , FlowNodeMode: 0      //1 表单录入 2审核  0 其他
    , WfFlowApply: null    //流程主表数据
    , MacrosData: null     //宏控件基础数据
    , init: function (type) {   //初始化页面
        FormUI.curRootPath = $("#RootPath").val();
        if ($("#FlowApplyID").val()) {
            FormUI.FlowApplyID = $("#FlowApplyID").val();
        }
        if ($("#FlowVID").val()) {
            FormUI.FlowVID = $("#FlowVID").val();
        }
        FormUI.FlowPage = GetQueryString('page') == null ? 0 : GetQueryString('page');
        FormUI.PageType = type; //获取页面展现方式（0 预览  1其他）
        if (FormUI.PageType == 0) {
            FormUI.FlowStatus = -1;
        }
        if (!FormUI.FlowVID && FormUI.PageType != 0) {
            return;
        }
        if (FormUI.PageType == 0) {  //预览
            FormUI.initFormControl();
        }
        else {
            FormUI.GetInitData();   //获取基础数据
        }
        FormUI.PreventBack();   //防止退格键返回
        FormUI.changeTab();
    }
    , GetInitData: function () {      //获取表单基础数据
        var _this = this;
        $.ajax({
            type: 'Post'
            , url: FormUI.curRootPath + "/WfFormDefine/GetFormData"
            , data: { 'FlowApplyID': _this.FlowApplyID, 'FlowVID': _this.FlowVID }
            , success: function (db) {
                if (db.flag == 1) {
                    if (db.data) {
                        FormUI.FlowNodeID = db.data.FlowNodeID;
                        FormUI.FlowIsDraft = db.data.IsDraft;
                        FormUI.FlowIsOpen = db.data.IsOpen;
                        FormUI.FormVID = db.data.FormVID,
                        FormUI.FlowApplyStepID = db.data.FlowApplyStepID
                        FormUI.FlowStatus = db.data.FlowStatus;
                        FormUI.FlowModel = db.data.FlowModel;
                        FormUI.BaseData = db.data;
                        FormUI.MacrosData = db.data.MacrosData;
                        FormUI.HandleShow();
                        FormUI.ChangeResult();
                        FormUI.ChangeFlowStep();
                        FormUI.LoadDraft();    //加载草稿信息

                        if (db.data.WfFlowApply != null && db.data.WfFlowApply.IsUrgency == 1) {
                            $('#ckUrgency').prop("checked", true);
                        }
                        if (FormUI.FlowStatus != 1 || FormUI.PageType != 1) {
                            $('#ckUrgency').attr('disabled', 'disabled');
                        }

                        if (FormUI.FlowStatus != -1 && FormUI.BaseData.WfFlowApply != null && !FormUI.BaseData.WfFlowApply.FlowVId) {
                            FormUI.AlertFrame(0);
                            return false;
                        }

                        if (FormUI.FlowStatus == 1) {
                            $("#FlowApplyName").attr('readonly', false);
                            var codetype = $("#PaperNO").attr("data-codetype");
                            if (codetype == 2) {
                                $("#PaperNO").attr('readonly', false);
                            }
                        }
                        if (FormUI.FlowIsOpen == 0) {
                            FormUI.AlertFrame(1);
                            return false;
                        }
                        if (FormUI.FlowPage == 2) {
                            if (FormUI.FlowApplyStepID == 0) {
                                FormUI.AlertFrame(0);
                                return false;
                            }
                            else if (FormUI.BaseData.WfFlowApply != null && FormUI.BaseData.WfFlowApply.FlowStatus == 3) {
                                FormUI.AlertFrame(0);
                                return false;
                            }
                        }
                    }
                    else {
                        if (FormUI.FlowStatus != -1) {
                            FormUI.AlertFrame(0);
                            return false;
                        }
                    }
                }
                else {
                    layer.alert(db.msg, { icon: 1 });
                    return false;
                }
                FormUI.initFormHtml();
                FormUI.showHandle();
            }
        });
    }
    , changeTab: function () {
        $('ul').on('click', 'li a[data-toggle=tab]', function () {
            $('ul li').removeClass("active");
            $(this).parent().addClass("active");
            $(".tab-pane").removeClass("in active");
            var tabpane = $(this).attr("alt");
            $(tabpane).addClass("active");
        });
    }
    , showHandle: function () {
        var _this = this;
        if ((_this.FlowStatus == 1) || (_this.FlowStatus == 2)) {
            $("#myTab").on("click", "li", function () {
                var title = $(this).find("a").attr("title");
                if (title == "流程进度") {
                    $("#formFooter").hide();
                }
                else if (title == "流程轨迹") {
                    $("#formFooter").hide();
                }
                else if (title == "流程列表") {
                    $("#formFooter").hide();
                }
                else {
                    $("#formFooter").show();
                }
                if (_this.FlowPage == 0) {
                    $("#formFooter").hide();
                }
            });
        }
        if (_this.FlowPage == 0) {
            $("#formFooter").hide();
        }
    }
    , initFormHtml: function () {     //加载表单的HTML
        if (FormUI.BaseData && FormUI.BaseData.initData) {
            var data = FormUI.BaseData.initData;
            if (data && data.length > 0) {
                var ids = "";
                for (var i = 0; i < data.length; i++) {
                    var navHtml = "", tabHtml = "";
                    if (FormUI.FlowStatus > 1) {
                        if (data[i].FormHtml) {
                            data[i].FormHtml = data[i].FormHtml.replace("../SFFormEditorNew/formdesign/images/imagebackground.jpg", "");
                        }
                    }
                    if (i == 0) {
                        navHtml = '<li class="active"><a data-toggle="tab" alt="#tab-content' + i + '" href="#">' + data[i].FormName + '</a>';
                        tabHtml = '<div id="tab-content' + i + '" class="tab-pane in active" SfpluginFormNumber="' + data[i].FormVId + '" style="overflow-x:auto;overflow-y:hidden;">';
                        if (data[i].FormSort == 1) {
                            $('#formFooter').hide();
                            tabHtml = tabHtml + '<iframe id="Sfpluginiframe' + data[i].FormVId + '" name="Sfpluginiframe' + data[i].FormVId + '" src="' + data[i].FormURL + '" style="border:0;overflow-x:auto;overflow-y:hidden;width:100%;"></iframe>';
                            $("#btnSave").attr("disabled", true);
                        }
                        else {
                            tabHtml = tabHtml + data[i].FormHtml;
                        }
                        tabHtml = tabHtml + '</div>';
                    }
                    else {
                        navHtml = navHtml + '<li><a data-toggle="tab" alt="#tab-content' + i + '" href="#">' + data[i].FormName + '</a>';
                        tabHtml = tabHtml + '<div id="tab-content' + i + '" class="tab-pane" SfpluginFormNumber="' + data[i].FormVId + '" style="overflow-x:auto;overflow-y:hidden;">' + data[i].FormHtml + '</div>';
                    }
                    $('.nav.nav-tabs').append(navHtml);
                    $('.tab-content').append(tabHtml);
                    ids = ids + data[i].FormVId + ",";
                    FormUI.initOffice(data[i].FormVId);   //office控件初始化

                    if (data[i].FormSort == 1 && data[i].FormURL) {
                        FormUI.IframeLoadAfter(data[i].FormVId);  //设置iframe中的字段可见和只读
                    }
                }
                var tablen = data.length;  //表单个数
                if (FormUI.FlowStatus == 1) {       //流程发送
                    if (FormUI.FlowModel == 1) {  //固定流程
                        if (!navigator.userAgent.match(/mobile/i)) {      //非手机端加载流程轨迹
                            FormUI.LoadFlowTrack(tablen);                  //固定流程轨迹
                        }
                    }
                }
                else {
                    FormUI.LoadFlowInfo(tablen);  //流程进度
                }
                increaseControlIDs = FormUI.GetIncreaseIds();    //获取自增行中控件ID集合
                FormUI.GetIncreaseIDList();                              //获取自增表格控件ID集
                FormUI.Createvalidation(FormUI.BaseData.FieldPower);     //动态创建js
                FormUI.initFormControl();                                //初始化控件 
            }
            else if (FormUI.PageType == 0) {    //预览
                FormUI.initFormControl();
            }
        }
        else {
            //FormUI.Createvalidation(FormUI.BaseData.FieldPower);      //动态创建js
            FormUI.initFormControl();
        }
    }
    , initFormControl: function () {    //初始化表单控件    
        FormUI.initIncrease2();    //自增行初始化
        FormUI.initMacro();       //宏控件初始化
        FormUI.initText();        //文本框初始化
        FormUI.initTextarea();    //文本区初始化
        FormUI.initRadio();       //单选框初始化
        FormUI.initCheckbox();    //复选框初始化
        FormUI.initSelects();     //下拉框初始化        
        FormUI.initDate();        //日期框初始化
        FormUI.initCalcula();     //计算框初始化
        FormUI.initImage();       //图片框初始化
        FormUI.initUpload();      //上传框初始化
        FormUI.initSql();         //SQL查询初始化        
        FormUI.initHtmlarea();    //Html编辑器初始化                
        if (FormUI.PageType == 0) {
            FormUI.initOffice();  //Office控件初始化   
        }
        FormUI.initAutonum();     //自动编号初始化
        FormUI.initStatis();      //统计框初始化 
        FormUI.initControlEvent() //事件初始化
    }
    , fillData: function (id, sfplugins) {   //填充数据
        var _this = this;
        if (_this.FlowStatus == 1 && _this.FlowIsDraft == 0) {
            return true;
        }
        if (!FormUI.BaseData || !FormUI.BaseData.FieldPower || FormUI.BaseData.FieldPower.length == 0) {
            return;
        }
        var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的权限
        if (!data && data.length > 0) {
            if (data[0].IsIncrease == 1) {
                return;
            }
        }

        if (FormUI.BaseData.showData && FormUI.BaseData.showData.length > 0) {
            var showdata = FormUI.FiltData(FormUI.BaseData.showData, "Key", id);  //获取控件值
            if (showdata && showdata.length > 0) {
                _this.fillControlData(id, sfplugins, showdata[0]);
            }
        }
    }
    , fillControlData: function (id, sfplugins, data) {
        switch (sfplugins) {
            case "text":           //文本框
                $("#" + id).val(data.Value);
                break;
            case "select":         //下拉框
                $("#" + id).val(data.Value);
                $("#" + id).attr("orgvalue", data.Value);
                $("#" + id).attr("value", data.Value);
                break;
            case "textarea":       //文本区
                $("#" + id).text(data.Value);
                break;
            case "radio":
            case "radios":         //单选框
                FormUI.showRadios(id, data);
                break;
            case "checkbox":
            case "checkboxs":      //复选框
                FormUI.showCheckboxs(id, data);
                break;
            case "autonumber":     //自动编号
                FormUI.showAutonum(id, data);
                break;
            case "date":           //日期控件
                FormUI.showDate(id, data);
                break;
            case "calculation":    //计算控件
                FormUI.showCalcula(id, data);
                break;
            case "image":         //图片控件
                FormUI.showImage(id, data);
                break;
            case "upload":         //上传控件
                FormUI.showUpload(id, data);
                break;
            case "statistics":     //统计控件
                FormUI.showStatis(id, data);
                break;
            case "htmlarea":     //Html编辑器
                FormUI.showHtmlarea(id, data);
                break;
            case "macros":     //Html编辑器
                FormUI.showMacros(id, data);
                break;
            case "userselect":     //人员选择框
                FormUI.showUserselect(id, data);
                break;
        }
    }
    , initText: function () {    //文本框
        $("input[sfplugins='text']").each(function (i, x) {
            FormUI.setText(x, 0);
        });
    }
    , initTextarea: function () {   //文本区
        $("textarea[sfplugins='textarea']").each(function (i, x) {
            FormUI.setTextarea(x, 0);
        });
    }
    , initRadio: function () {   //单选框
        $("span[sfplugins='radios']").each(function (i, x) {
            FormUI.setRadio(x, 0);
        });
    }
    , initCheckbox: function () {  //复选框
        $("span[sfplugins='checkboxs']").each(function (i, x) {
            FormUI.setCheckbox(x, 0);
        });
    }
    , initSelects: function () {    //下拉框
        $("select[sfplugins='select']").each(function (i, x) {
            FormUI.setSelects(x, 0);
        });
    }
    , initAutonum: function () {   //自动编号框    
        var _this = this;
        $("input[sfplugins='autonumber']").each(function (i, x) {
            FormUI.setAutonum(x, 0);
        });
    }
    , initDate: function () {      //时间控件
        $("input[sfplugins='date']").each(function (i, x) {
            FormUI.setDate(x, 0);
        });
    }
    , initCalcula: function () {   //计算控件
        $("input[sfplugins='calculation']").each(function (i, x) {
            FormUI.setCalcula(x, 0);
        });
    }
    , initImage: function () {
        $("input[sfplugins='image']").each(function (i, x) {
            FormUI.setImage(x, 0);
        });
    }
    , initUpload: function () {
        $("input[sfplugins='upload']").each(function (i, x) {
            FormUI.setUpload(x, 0);
        });
    }
    , initSql: function () {
        $("input[sfplugins='sqlquery']").each(function (i, x) {
            FormUI.setSql(x, 0);
        });
    }
    , initStatis: function () {
        $("input[sfplugins='statistics']").each(function (i, x) {
            FormUI.setStatis(x, 0);
        });
    }
    , initHtmlarea: function () {
        $("input[sfplugins='htmlarea']").each(function (i, x) {
            FormUI.setHtmlarea(x, 0);
        });

        //$("input[sfplugins='htmlarea']").each(function (i, x) {//若是编辑进入 则将之前保存内容写入 span 再构建Html编辑器

        //});
    }
    , initMacro: function () {
        $("input[sfplugins='macros']").each(function (i, x) {
            FormUI.setMacros(x, 0);
        });
    }
    , getMacroBaseData: function () {
        if ($("input[sfplugins='macros']").length > 0) {
            var param = { type: 'Get', url: FormUI.curRootPath + '/WfFormDefine/GetMacroData?er=1' };
            param.callback = function (result) {
                if (result.flag == 1) {
                    var data = result.data;
                    FormUI.MacrosData = data;
                } else {
                    layer.msg(data.Msg);
                }
            }
            FormUI.GetAjaxData(param);
        }
    }
    , initOffice: function (FormVID) {    //Office控件
        $("input[sfplugins='office']").each(function (i, x) {
            FormUI.setOffice(x, 0);
        });
    }
    , initIncrease: function () {
        var _this = this;
        $("table tr[sfplugins='increasetable']").each(function (i, x) {
            if ($(x).find("td:last-child").html() == "<br>") {
                $(x).find("td:last-child").html("");
            }
            $(x).find("td:last-child")
                .append('<button type="button" class="increasetableadd" title="增加"><span class="glyphicon glyphicon-plus"></span></button>')
                .css("text-align", "left");
            if (FormUI.PageType == 1 && _this.FlowStatus == 0) {   //查看
                return;
            }
            var id = $(x).attr("id");
            if (_this.FlowStatus == -1 || _this.FlowModel == 1 || (_this.FlowModel == 2 && _this.FlowStatus == 1) || (_this.FlowModel == 2 && _this.FlowNodeID.indexOf("Input") > -1)) {
                //setTimeout("FormUI.AddTableRow('" + id + "')", 1000);
                FormUI.AddTableRow(id);
                FormUI.DelTableRow(x);       //自增表格删除行
            }

            FormUI.SetControlIsRead(id);
        });
        if (FormUI.FlowStatus != -1) {  //非预览
            FormUI.showIncrease(FormUI.BaseData.showData);   //自增行数据初始化
        }
        $("[sfplugins='increasetable']").each(function (i, x) {
            var id = $(x).attr("id");
            $("tr[id=" + id + "]>td").find("[sfplugins]").each(function (j, y) { //自增表格模板列
                var sfplugins = $(y).attr("sfplugins");
                if (sfplugins == "date") {
                    $(y).val("");
                }
                else if (sfplugins == "calculation") {
                    FormUI.setCalcula(y, 2);
                }
                $(y).attr("increasecontrol", "1");
            });
            var increasehtml = null;
            if (FormUI.BaseData != null && FormUI.BaseData.showData != null && FormUI.BaseData.showData.length > 0) {
                increasehtml = FormUI.FiltData(FormUI.BaseData.showData, "Key", id);
            }
            if (increasehtml == null || increasehtml == "" || increasehtml == undefined || increasehtml.length == 0) {
                FormUI.initIncreaseTrControl(id, 1, 0);
            }
        });
    }
    , initIncrease2: function () {
        var _this = this;
        $("table tr[sfplugins='increasetable']").each(function (i, x) {
            var id = $(x).attr("id");
            if ($(x).find("td:last-child").html() == "<br>") {
                $(x).find("td:last-child").html("");
            }
            //初始化按钮
            $(x).find("td:last-child")
                .append('<button type="button" class="increasetableadd" title="增加"><span class="glyphicon glyphicon-plus"></span></button>')
                .css("text-align", "left");
            FormUI.AddTableRow(id); //自增行表格添加
            FormUI.DelTableRow(x);  //自增表格删除行  
            FormUI.refreshIncreaseTemplate(id);     //刷新模板列
            FormUI.initIncreaseTrControl(id, 1, 0)  //初始化模板列控件
            if (FormUI.FlowStatus != -1) {  //非预览
                FormUI.showIncrease2(id);  //填充数据
                FormUI.SetControlIsRead(id); //设置是否可见或编辑
            }
        });
    }
    , refreshIncreaseTemplate: function (id) {
        $("tr[id=" + id + "]>td").find("[sfplugins]").each(function (j, y) { //自增表格模板列
            var sfplugins = $(y).attr("sfplugins");
            if (sfplugins == "date") {
                $(y).val("");
            }
            else if (sfplugins == "calculation") {
                FormUI.setCalcula(y, 2);
            }
            $(y).attr("increasecontrol", "1");
        });
    }
    , initControlEvent: function () {
        //获取有设置事件属性的元素
        var all = $("[sfplugins][orgeventbind]");
        var t1 = $("select[orgeventbind]");
        var t2 = $("input[orgeventbind]");
        var t3 = $("textarea[orgeventbind]");
        var t4 = $("span[orgeventbind]");
        //解析事件属性绑定信息
        var GetBindInfo = function (elems) {
            var result = [];
            $(elems).each(function (idx, item) {
                var id = item.id;
                var bindstrs = $(item).attr("orgeventbind").split(',');
                $.each(bindstrs, function (jdx, jtem) {
                    var info = jtem.split('|');
                    result.push({ id: id, eventName: info[0], eventFn: info[1] });
                });
            })
            return result;
        }
        //合并属性绑定信息
        var s1 = GetBindInfo(t1),
        s1 = s1.concat(GetBindInfo(t2)),
        s1 = s1.concat(GetBindInfo(t3)),
        s1 = s1.concat(GetBindInfo(t4));
        //绑定事件
        $.each(s1, function (idx, item) {
            $("#" + item.id).on(item.eventName, function () {
                eval(item.eventFn + "()");
                eval("var fn=" + item.eventFn + "();");

            });
        });

    }
    , setText: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        var increasecontrol = $(x).attr("increasecontrol");
        $(x).removeAttr("readonly");
        $(x).removeAttr("onfocus");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var orgtype = $(x).attr('orgtype');
        if (orgtype && orgtype == 'float') {
            var number = $(x).attr('orgfigure');  //小数位数
            $(x).on('keyup', function (event) {
                var val = $(x).val();
                $(x).val(_this.RadixLimit(val, number));
            });
        }
        //FormUI.VerifyLength(id, "text");  //验证长度限制
        FormUI.AddMustSign(x);
        FormUI.SetControlIsRead(id);
        if (increasecontrol != 1) {
            FormUI.fillData(id, "text");   //填充数据 
        }
    }
    , setTextarea: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        $(x).removeAttr("readonly");
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        FormUI.AddMustSign(x);
        //FormUI.VerifyLength(id, "textarea");  //验证长度限制
        FormUI.SetControlIsRead(id);
        if (increasecontrol != 1) {
            FormUI.fillData(id, "textarea");
        }
    }
    , setRadio: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        FormUI.AddMustSign(x);
        FormUI.BindData(id, "radios");  //绑定数据      
    }
    , setCheckbox: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        FormUI.AddMustSign(x);            //添加必填标示
        FormUI.BindData(id, "checkbox");  //绑定数据
    }
    , setSelects: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        //$(x).addClass('select-select2').select2();
        FormUI.AddMustSign(x);                     //添加必填标示            
        FormUI.BindData(id, "select");
    }
    , setAutonum: function (x, kind) {
        var _this = this;
        var id = $(x).attr("id");
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的属性 
        if (FormUI.PageType == 1) {
            if (data.length > 0 && data[0].IsIncrease == 1 && _this.FlowStatus != 1) {
                return true;
            }
        }
        if (data != null && data.length > 0) {
            $(x).attr("orgFormVId", data[0].FormVId);
        }
        FormUI.ProAutonum(x);              //生成编号
        FormUI.SetControlIsRead(id);       //设置控件的权限
        if (increasecontrol != 1) {
            FormUI.fillData(id, "autonumber"); //填充数据
        }
    }
    , setAutonum2: function (x, itemcount) {  //自增表格中的自动编号
        var _this = this;
        var strid = $(x).attr("id");
        var figures = $(x).attr("orgnumfigures");
        var begin = 0;
        $("#" + strid + "number").attr("name", strid + "numberItem" + itemcount);
        $("#" + strid + "number").attr("id", strid + "numberItem" + itemcount);
        FormUI.ProAutonum(x);
        if (figures != "") {
            var itemauto = $("[id^=" + strid + "Item]");
            if (itemauto && itemauto.length > 0) {
                itemauto.each(function (j, y) {  //获取最大的序号
                    var strbegin = $(y).attr("begin");
                    if (parseInt(strbegin) > parseInt(begin)) {
                        begin = strbegin;
                    }
                });
                if (parseInt(itemcount) > parseInt(begin)) {
                    begin = parseInt(itemcount)
                }
            }
            var num = $("#" + strid).val();
            var fig = FormUI.PrefixInteger(parseInt(begin) + 1, parseInt(figures));
            num = num.substr(0, num.length - figures) + fig;
            $("#" + strid).val(num);
            $("lable[name='" + strid + "number']:last").html(num);
            //$("lable[name='" + strid + "number']:first").remove();
            $("#" + strid).attr("begin", parseInt(begin) + 1);
        }
    }
    , setDate: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        FormUI.AddMustSign(x);
        $(x).val('');
        var id = $(x).attr("id");
        if ($(x).attr("orgdatetimetype") == "2") {
            $(x).val(FormUI.GetCurrenData());
            $(x).attr("disabled", "disabled");
            $(x).after("<input id='" + id + "Temp' name='" + id + "' type='hidden' value='" + $(x).val() + "' />");
        }
        FormUI.SetControlIsRead(id); //设置控件的权限
        if (increasecontrol != 1) {
            FormUI.fillData(id, "date"); //填充数据
        }
    }
    , setCalcula: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var preObj = $(x);
        var formul = $(x).attr("orgformulaval");
        var id = $(x).attr("id");
        var reg = new RegExp("[+-//*()]"); //匹配运算符号
        var opt = formul.split(reg);
        for (var i = 0; i < opt.length; i++) {
            if (!$("#" + opt[i])) {//假如其中一个控件无法找到 则跳出
                return;
            }
            if (!opt[i]) {
                continue;
            }
            if (!$("#" + opt[i]).data("events") || !$("#" + opt[i]).data("events")["blur"]) {
                var sfplugins = $("#" + opt[i]).attr("sfplugins");

                if (sfplugins == "calculation") {
                    if (FormUI.PageType == 0) {  //预览
                        $("#htmlContent").on("change", "[id='" + opt[i] + "']", function () {
                            FormUI.Calculate(id);
                        });
                    }
                    else {
                        $("div.tab-pane").on("change", "[id='" + opt[i] + "']", function () {
                            FormUI.Calculate(id);
                        });
                    }
                }
                else {
                    if (FormUI.PageType == 0) {
                        $("#htmlContent").on("blur", "[id='" + opt[i] + "']", function () {
                            FormUI.Calculate(id);
                        });
                    }
                    else {
                        $("div.tab-pane").on("blur", "[id='" + opt[i] + "']", function () {
                            FormUI.Calculate(id);
                        });
                    }
                }
            }
        }
        $(x).val("");
        $(x).hide();
        $(x).attr("relid", id + "calcula");
        $(x).parent().find('#' + id + 'calcula').remove();
        preObj.after("<span id='" + id + "calcula' name='" + id + "calcula' style='color:" + $(x).attr('orgfontcolor') + "'></span>");
        if (opt.length > 0) {
            FormUI.Calculate(id);
        }
        FormUI.SetControlIsRead(id);        //设置控件的权限
        if (kind != 2 && increasecontrol != 1) {
            FormUI.fillData(id, "calculation"); //填充数据
        }
    }
    , setImage: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (!increasecontrol) {
            increasecontrol = 0;
        }
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var preObj = $(x).parent();
        var id = $(x).attr("id");
        var html = "<div id='" + id + "preview'>";
        if (FormUI.FlowStatus == 1 || FormUI.PageType == 0) {
            html += "<img id='" + id + "imageName' border=0 src='" + FormUI.curRootPath + "/content/scripts/workflow/uedit/formdesign/images/imagebackground.jpg' style='width:400px;height:300px;' />";
        }
        else {
            html += "<img id='" + id + "imageName' border=0 src='' style='width:400px;height:300px;' />";
        }
        html += "</div>";
        html += "<input id='" + id + "imagePath' name='" + id + "imagePath' type='hidden'>"
        var orgisrequire = $(x).attr("orgisrequire");
        if (orgisrequire == "true") {
            html += "&nbsp;<span for='" + id + "' style='color:red;font-size:16px;'>*</span>";
        }
        html += "<input id='" + id + "' name='" + id + "' type='file' sfplugins='image' increasecontrol='" + increasecontrol + "'  onchange='FormUI.AddImage(this);' style='display:inline;' />";
        $(x).after(html);
        var orgtype = $(x).attr("orgtype");
        $("#" + id + "imageName").attr("orgtype", orgtype);
        if (orgtype == "Preinstall") {   //预设图片大小
            $("#" + id + "imageName").css("width", $(x).attr("orgwidth"));
            $("#" + id + "imageName").css("height", $(x).attr("orgheight"));
            $("#" + id + "imageName").attr("orgwidth", $(x).attr("orgwidth"));
            $("#" + id + "imageName").attr("orgheight", $(x).attr("orgheight"));
            $("#" + id + "imageName").attr("orgwidthtype", $(x).attr("orgwidthtype"));
        }
        $(x).remove();
        $("#" + id).attr("sfplugins", "image");
        FormUI.SetControlIsRead(id);  //设置控件的权限
        if (increasecontrol != 1) {
            FormUI.fillData(id, "image"); //填充数据
        }
    }
    , setUpload: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var id = $(x).attr("id")
           , orgisrequire = $(x).attr("orgisrequire")
           , orgwidth = $(x).attr("orgwidth")
           , orgwidthtype = $(x).attr("orgwidthtype");
        var width = orgwidth + (orgwidthtype == "percent" ? "%;" : "px;");
        var html = '<div id="div' + id + '" style="display: inline;"><input type="hidden" id="' + id + 'filePath" name="' + id + 'filePath"/>';                //存放文件路径
        html = html + '<input type="button" style="width:' + width + '" class="btn btn-default sf-bt" id="btnSelect' + id + '" for="' + id + '" value="上传文件" onclick="FormUI.clickUpload(this)" />';
        if (orgisrequire == "true") {
            html = html + '<span for="' + id + '" style="color:red;font-size:16px;">*</span>';
        }
        html = html + '</div>'
        $(x).hide().after(html);
        $(x).attr('onchange', 'FormUI.GetUploadFilePath(this);');
        FormUI.SetControlIsRead(id);  //设置控件的权限
        if (increasecontrol != 1) {
            FormUI.fillData(id, "upload"); //填充数据
        }
    }
    , clickUpload: function (x) {
        var id = $(x).attr('for');
        $("#" + id).click();
    }
    , setSql: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var val = $(x).attr("orgsql");
        if (val.match(new RegExp('\"'))) {
            layer.msg("请使用单引号代替双引号！");
            return;
        }
        if (!val) {
            layer.msg('请输入SQL语句！');
            return;
        }
        //var param = { type: "Post", url: "FormAction.ashx?rad=" + Math.random(), data: { 'querycontent': encodeURI(val), 'action': 'querysql' } };
        var param = { type: "Post", url: FormUI.curRootPath + "/WfFlowSend/QueryContent", data: { 'querycontent': encodeURI(val) } };
        param.callback = function (result) {
            if (result.flag == 1) {
                var data = result.data;
                FormUI.InitTable($(x), eval(data.TbCols), eval(data.TbData));
            } else {

            }
        }
        FormUI.GetAjaxData(param);
        FormUI.SetControlIsRead($(x).attr("id"));                //设置控件的权限
    }
    , setStatis: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        $(x).val("");
        var id = $(x).attr("id");
        $(x).hide();
        $(x).after("<span name='" + id + "statistics' id='" + id + "statistics'></span>");
        FormUI.SetControlIsRead(id);       //设置控件的权限
        if (increasecontrol != 1) {
            FormUI.fillData(id, "statistics"); //填充数据
        }
    }
    , setHtmlarea: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (!increasecontrol) {
            increasecontrol = 0;
        }
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var id = $(x).attr("id");
        var editorid = id + 'editor';
        var height = $(x).css("height");
        var width = $(x).css("width");
        $(x).after('<span id="' + editorid + '" style="width:' + width + ';height:' + height + ';"> </span>');
        $(x).after("<input sfplugins='htmlarea' type='hidden' id='" + id + "' increasecontrol='" + increasecontrol + "' name='" + id + "' style='width:" + width + ";height:" + height + "'; relId='" + editorid + "' orgwidth='" + width + "' orgheight='" + height + "'  value=''   />");
        $(x).remove();

        FormUI.SetControlIsRead(id);     //设置控件的权限
        FormUI.fillData(id, "htmlarea"); //填充数据
        var relid = $("#" + id).attr("relid");
        var height = $("#" + id).attr("orgheight");
        var width = $("#" + id).attr("orgwidth");
        height = height.substr(0, height.length - 2);
        width = width.substr(0, width.length - 2);
        var htmlEditor = UE.getEditor(relid, {
            newId: 'sf',
            toolsf: true,//是否显示，设计器的 toolbars
            textarea: 'design_content',
            //focus时自动清空初始化时的内容
            //autoClearinitialContent:true,
            //关闭字数统计
            wordCount: false,
            //关闭elementPath
            elementPathEnabled: false,
            readOnly: true,
            //默认的编辑区域高度
            initialFrameHeight: height,
            initialFrameWidth: width
            //iframeCssUrl: "bootstrap.css" //引入自身 css使编辑器兼容你网站css
            //更多其他参数，请参考ueditor.config.js中的配置项
        });
        FormUI.setHtmlDisabled(id, htmlEditor); //设置Html编辑器不可编辑 
    }
    , setHtmlDisabled: function (id, htmlEditor) {
        if (!FormUI.BaseData || !FormUI.BaseData.FieldPower || FormUI.BaseData.FieldPower.length == 0) {
            return;
        }
        var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的权限
        if (data != null && data.length > 0) {
            var IsReadOnly = data[0].IsReadOnly;  //是否只读
            var IsVisible = data[0].IsVisible;   //是否可见
            if (IsVisible && IsReadOnly) {
                var count = 0;
                var htmldisabled = setInterval(function () {
                    count = count + 1;
                    if (count >= 50) {
                        clearInterval(htmldisabled);
                    }
                    if (htmlEditor.body) {  //编辑器以加载完成才能设置为不可编辑
                        htmlEditor.setDisabled();
                        clearInterval(htmldisabled);
                    }
                }, 100);
            }
        }
    }
    , setMacros: function (x, kind) {
        var _this = this;
        if (FormUI.MacrosData != null) {
            FormUI.loadMacros(x, kind);
        }
        else {
            var param = { type: 'Get', url: FormUI.curRootPath + '/WfFormDefine/GetMacroData' };
            param.callback = function (result) {
                if (result.flag == 1) {
                    var data = result.data;
                    FormUI.MacrosData = data;
                    FormUI.loadMacros(x, kind);
                } else {
                    layer.msg(data.Msg);
                }
            }
            FormUI.GetAjaxData(param);
        }
    }
    , loadMacros: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        if (increasecontrol != 1) {
            increasecontrol = 0;
        }
        var type = $(x).attr('orgtype');
        var oNode = null;
        var orgisrequire = $(x).attr("orgisrequire");
        var id = $(x).attr("id");
        var data = FormUI.MacrosData;
        if (type == "1") {       //当前用户
            oNode = $("<lable>", { 'name': id });
            if (data && data.WfMacroUserInfo && data.WfMacroUserInfo.UserSystemId) {
                oNode.html(data.WfMacroUserInfo.UserSystemId);
                $(x).val(data.WfMacroUserInfo.UserSystemId).hide();
            }
            $(x).after(oNode);
        }
        else if (type == "2") {  //人员选择
            $(x).removeAttr("readonly");
            $(x).attr("useridlist", "");
            var width;
            var id = $(x).attr("id");
            if ($(x).attr('orgwidthtype') == "pixel") {
                width = ($(x).attr('orgwidth') - 40) + "px";
            }
            else {
                width = $(x).attr('orgwidth') + "px";
            }
            var userHtml = '<input type="text" id="' + id + 'UserName" readonly="readonly" style="width:' + width + ';display:inline-block;margin-right:5px;" class="form-control" />';
            userHtml += '<input type="hidden" id="' + id + 'UserCode"/>';
            userHtml += '<input type="button" id="' + id + 'selectButton" class="btn btn-default" onclick="javescript:FormUI.UserDialog(\'' + id + '\')" value="选择">';
            userHtml += '<input type="hidden" id="' + id + '" name="' + id + '" sfplugins="macros" increasecontrol="' + increasecontrol + '" orgtype="2" /> ';
            if (orgisrequire == "true") {
                userHtml += '<span style="color:red;font-size:16px;">*</span>';
            }
            $(x).before(userHtml);
            $(x).remove();
        }
        else if (type == "3" || type == "4") {  //部门选择 角色选择
            oNode = $("<select>", {
                'sfplugins': 'macros'
                , 'name': id
                , 'id': id
                , 'style': $(x).attr('style')
                , 'title': $(x).attr('title')
                , 'value': $(x).attr('value')
                , 'orgtype': $(x).attr('orgtype')
                , 'orgisrequire': $(x).attr('orgisrequire')
                , 'orgfontsize': $(x).attr('orgfontsize')
                , 'orgfontcolor': $(x).attr('orgfontcolor')
                , 'orgfontfamily': $(x).attr('orgfontfamily')
                , 'orgbordercolor': $(x).attr('orgbordercolor')
                , 'orgborderstyle': $(x).attr('orgborderstyle')
                , 'orgborderwidth': $(x).attr('orgborderwidth')
                , 'orgalign': $(x).attr('orgalign')
                , 'orgwidth': $(x).attr('orgwidth')
                , 'orgwidthtype': $(x).attr('orgwidthtype')
            });
            $(x).before(oNode);
            if (orgisrequire == "true") {
                $(x).before("&nbsp;<span style='color:red;font-size:16px;'>*</span>");
            }
            $(x).remove();
            var html = '<option value="">    </option>';
            if (data) {
                if (type == 3)  //部门列表
                {
                    if (data.WfMacroDeptList) {
                        html += FormUI.DeptList(data.WfMacroDeptList);
                    }
                }
                else {          //角色列表
                    if (data.WfMacroRoleList) {
                        html += FormUI.RoleList(data.WfMacroRoleList);
                    }
                }
            }
            $("#" + id).append(html);
        }
        else if (type == "5") {  //发起人姓名
            oNode = $("<lable>", { 'name': id });
            if (data && data.WfMacroUserInfo && data.WfMacroUserInfo.UserName) {
                oNode.html(data.WfMacroUserInfo.UserName);
                $(x).val(data.WfMacroUserInfo.UserName).hide();
            }
            $(x).after(oNode);
        }
        else if (type == "6") {  //发起人部门
            oNode = $("<lable>", { 'name': id });
            if (data && data.WfMacroUserInfo && data.WfMacroUserInfo.OrganizationName) {
                oNode.html(data.WfMacroUserInfo.OrganizationName);
                $(x).val(data.WfMacroUserInfo.OrganizationName).hide();
            }
            $(x).after(oNode);

        }

        FormUI.SetControlIsRead(id);   //设置控件的权限
        if (kind != 1 || type != 2) {
            FormUI.fillData(id, "macros"); //填充数据
        }
    }
    , setOffice: function (x, kind) {
        var _this = this;
        var increasecontrol = $(x).attr("increasecontrol");
        if (increasecontrol == 1 && kind == 0) {
            return;
        }
        var preObj = $(x).parent();
        var width = $(x).css("width");
        var height = $(x).css("height");
        var templateId = $(x).attr("orgtemplate");
        var id = $(x).attr("id");
        var IsVisible = "", IsReadOnly = "", objectID = "";
        var m = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);
        if (m && m.length > 0) {
            IsVisible = m[0].IsVisible == false ? 0 : 1;    //可见
            IsReadOnly = m[0].IsReadOnly == true ? 1 : 0;  //是否只读
            objectID = m[0].Id;
        }
        var url = FormUI.curRootPath
            + "/WfDocTemplate/Open?Id=" + templateId
            + "&TempName=" + id
            + "&FlowApplyID=" + _this.FlowApplyID
            + "&FlowApplyStepID=" + _this.FlowApplyStepID
            + "&ObjectID=" + objectID
            + "&IsVisible=" + IsVisible
            + "&IsReadOnly=" + IsReadOnly
            + "&Hieght=" + height
            + "&Width=" + width;
        var node = $('<div>', { 'id': id + 'div', 'style': 'width:' + width + ';height:' + height + ';' });
        node.append('<iframe sftype="office" id="' + id + 'iframe" name="' + id + 'iframe" src="' + url + '" frameborder="0"  width="100%" height="100%"></iframe>');
        preObj.append("<div style='cursor:pointer;color:red' onclick='FormUI.ToggleOffice(this)' forDiv='" + id + "div'>正文(点击展开/收缩)</div>")
        preObj.append(node);
        preObj.append('<a forDiv="' + id + '" orgtemplate="' + templateId + '" href="#" onclick="FormUI.DownloadOffice(\'' + templateId + '\',\'' + _this.FlowApplyID + '\',\'' + objectID + '\')">下载OFFICE文档内容</a>');
        $(x).remove();
        FormUI.SetControlIsRead(id);                //设置控件的权限
        FormUI.fillData(id, "office"); //填充数据
    }
    , showRadios: function (id, data) {  //单选框
        var radObj = $("#" + id).find("input[name='" + data.Key + "'][value='" + data.Value + "']");
        $("#" + id).find("input[type=radio][name=" + data.key + "]").prop("checked", false);
        radObj.prop("checked", true);
    }
    , showCheckboxs: function (id, data) {  //复选框
        var obj = $("#" + id);
        obj.find("input[type=checkbox][name=" + data.Key + "]").prop("checked", false);
        if (data.Value.indexOf(',') > 0) {
            var strVal = data.Value.split(',');
            for (var j = 0; j < strVal.length; j++) {//多个选中值
                var chkObj = obj.find("input[name='" + data.Key + "'][value='" + strVal[j] + "']");
                chkObj.prop("checked", true);
            }
        } else {
            var chkObj = obj.find("input[name='" + data.Key + "'][value='" + data.Value + "']");
            chkObj.prop("checked", true);
        }
    }
    , showAutonum: function (id, data) {   //自动编号
        var obj = $("#" + id);
        obj.val(data.Value);
        id = data.Key + "number";
        var oNode = $("<lable>", { 'name': id, 'id': id });   //生成新的编号展示lable
        oNode.html(data.Value);
        $("#" + id).remove();       //删除之前的自动编号
        $(obj).after(oNode);        //插入新的编号展示lable
        oNode = $("<input>", { 'type': 'hidden', 'id': data.Key, 'sfplugins': 'autonumber', 'name': data.Key, 'value': data.Value });
        $(obj).remove();            //删除旧的自动编号input
        $("#" + id).before(oNode);  //插入新的自动编号框
    }
    , showDate: function (id, data) {   //日期控件
        var obj = $("#" + id);
        obj.val(data.Value);
        obj.attr("value", data.Value);  //打印时取值
        if (obj.attr("orgdatetimetype") == "2") {
            $("#" + id + "Temp").val(obj.val());
        }
    }
    , showCalcula: function (id, data) {
        $("#" + id).val(data.Value);
        $("#" + id + "calcula").text(data.Value);
    }
    , showImage: function (id, data) {   //图片控件
        var _this = this;
        var host = window.location.protocol + "//" + window.location.host;
        var imgSrc = "";
        if (data.Value == "" || data.Value == null || data.Value == "undefined") {
            imgSrc = FormUI.curRootPath + '/Content/Scripts/Workflow/uedit/formdesign/images/imagebackground.jpg';
        }
        else {
            if (data.Value.indexOf('.') >= 0) {
                imgSrc = FormUI.curRootPath + '/PublicFile/FormAttachment/' + data.Value;
            }
            else {
                imgSrc = FormUI.curRootPath + '/CsFile/Download/' + data.Value;
            }
        }
        var orgtype = $("#" + id + "imageName").attr("orgtype");
        var width, height, orgwidth, orgheight, orgwidthtype;

        if (orgtype == "Preinstall") {   //预设图片大小
            var s;
            orgwidthtype = $("#" + id + "imageName").attr("orgwidthtype");
            if (orgwidthtype == "pixel") {
                s = "px";
            }
            else {
                s = "%";
            }
            orgwidth = $("#" + id + "imageName").attr("orgwidth");
            orgheight = $("#" + id + "imageName").attr("orgheight");
            width = orgwidth + s;
            height = orgheight + s;
        }
        if (orgtype == "Preinstall") {   //预设图片大小
            $("#" + id + "imageName").css("width", width);
            $("#" + id + "imageName").css("height", height);

            $("#" + id + "imageName").attr("orgwidth", orgwidth);
            $("#" + id + "imageName").attr("orgheight", orgheight);
            $("#" + id + "imageName").attr("orgwidthtype", orgwidthtype);
        }
        else {
            $("#" + id + "imageName").removeAttr("style");
        }
        $("#" + id + "imageName").attr("src", imgSrc);
        $("#" + id + "imagePath").val(data.Value);

        if (_this.FlowStatus == 0) {
            $("#" + id).hide();
        }
    }
    , showUpload: function (id, data) {   //上传控件
        var host = window.location.protocol + "//" + window.location.host;
        var preObj = $("#" + id).parent();
        FormUI.LoadFileList(data);
    }
    , showStatis: function (id, data) {  //显示统计数据
        $("#" + id).val(data.Value);
        $("#" + id + "statistics").text(data.Value);
    }
    , showHtmlarea: function (id, data) {  //Html编辑器
        var relId = $("#" + id).attr("relId");
        $("#" + relId).html(data.Value);
    }
    , showMacros: function (id, data) {  //显示宏数据
        var type = $("#" + id).attr('orgtype');
        if (type == "1" || type == "5" || type == "6") {
            $("lable[name=" + id + "]").html(data.Value);
        }
        else if (type == 2) {  //选人控件
            var strVal = data.Value.split("_");
            if (strVal.length == 2) {
                $("#" + data.Key + 'UserCode').val(strVal[0]);
                $("#" + data.Key + 'UserName').val(strVal[1]);
                $("#" + data.Key).val(data.Value);
            }
        }

        $("#" + id).val(data.Value);
    }
    , showIncrease: function (data) {    //自增行数据展示
        var _this = this;
        var ids = "";
        $("[sfplugins='increasetable']").each(function (i, x) {                   //自增表格
            var id = $(x).attr("id");
            var html = null;

            if (data != null && data.length > 0) {
                var increasehtml = FormUI.FiltData(data, "Key", id);
                if (increasehtml && increasehtml.length > 0) {
                    html = increasehtml[0].Value;
                }
            }
            else {
                html = $(x).prop("outerHTML");
            }

            if (html == undefined || html == "") {
                return;
            }
            if (_this.FlowStatus == 0) {  //查看
                $(x).hide();
            }
            if (html) {
                try {
                    $(x).prop("outerHTML", html);
                }
                catch (ex) {   //IE9不支持outerHTML赋值
                    $(x).wrap("<div id='tempIncrease'></div>");
                    $("div#tempIncrease").after(html).remove();
                }
                $(html).find('[sfplugins="calculation"]').each(function (k, z) {
                    var tId = $(z).attr('Id');
                    $('#' + tId + 'calcula').html('');
                });


                $("input.increasetableadd").replaceWith('<button type="button" class="increasetableadd" title="增加"><span class="glyphicon glyphicon-plus"></span></button>');
                $("input.increasetabledel").replaceWith('<button type="button" class="increasetabledel" title="删除"><span class="glyphicon glyphicon-minus"></span></button>');
                if (data != null && data.length > 0) {
                    $("tr[id=" + id + "]>td").find("[sfplugins]").each(function (j, y) { //自增表格模板列
                        var templateid = $(y).attr("id");
                        ids = ids + templateid + ";";
                        var templatevalue = FormUI.FiltArray(data, "Key", templateid);
                        if (templatevalue.length > 0) {
                            $("tr[id^=" + id + "Sub]>td").find("[id^=" + templateid + "Item][sfplugins-item]").each(function (k, z) {
                                var itemid = $(z).attr("id");
                                var sfplugins = $(z).attr("sfplugins-item");
                                var itemvalue = templatevalue[k].Value;

                                if (sfplugins == "upload") {
                                    $("#" + itemid + "fileList a:contains('删除')").parent().hide();  //隐藏删除按钮
                                }
                                else if (sfplugins == "image") {
                                    $("#" + itemid + "imagePath").val(itemvalue);
                                    $("#" + itemid).hide();
                                }
                                else if (sfplugins == "radios") {
                                    $("input[name='" + itemid + "']").attr("checked", false);
                                    $("input[name='" + itemid + "'][value=" + itemvalue + "]").attr("checked", true);
                                }
                                else if (sfplugins == "checkboxs") {
                                    if (itemvalue && itemvalue != "") {
                                        var ck = itemvalue.split(',');
                                        $("input[name='" + itemid + "']").attr("checked", false);
                                        $("input[name='" + itemid + "']").each(function (k, z) {
                                            var v = $(z).attr("value");
                                            if ($.inArray(v, ck)) {
                                                $(z).attr("checked", true);
                                            }
                                        });
                                    }
                                }
                                else if (sfplugins == "macros") {
                                    var orgtype = $('#' + itemid).attr('orgtype');
                                    if (orgtype == 2)   //选人
                                    {
                                        if (itemvalue != '' && itemvalue.indexOf('_') > -1) {
                                            $('#' + itemid + 'UserName').val(itemvalue.split('_')[1]);
                                        }
                                    }
                                    $("#" + itemid).val(itemvalue);

                                }
                                else if (sfplugins == "userselect") {
                                    var uvalue = itemvalue.split("_");
                                    $("#" + itemid).val(itemvalue);
                                    $("#" + itemid + "macros").attr("useridlist", uvalue[0]);
                                    $("#" + itemid + "macros").val(uvalue[1]);
                                    $("#" + itemid + "code").val(uvalue[0]);
                                }
                                else {
                                    $("#" + itemid).val(itemvalue);
                                }
                            });
                        }
                    });
                }
            }
        });
        FormUI.SetIncreaseIsRead(ids);   //设置自增行的只读属性
        return ids;
    }
    , showIncrease2: function (id) {
        var _this = this;
        if (id) {
            var fields = [];
            $('#' + id).find('[sfplugins]').each(function (j, y) {  //获取自增行的控件
                fields.push({ id: $(y).attr('id'), sfplugins: $(y).attr('sfplugins') });  //获取控件信息
            });
            var fieldsValue = _this.GetIncreaseData(fields);
            if (fieldsValue && fieldsValue.length > 0) {  //自增行内的值存在
                for (var j = 0; j < fieldsValue.length; j++) {
                    $.each(fieldsValue[j], function (k, z) {
                        var fieldcontrol = _this.FiltData(fields, 'id', k);
                        _this.fillControlData(k, fieldcontrol[0].sfplugins, { Key: k, Value: z });
                    });
                    $('tr[id="' + id + '"] button[type=button].increasetableadd').click();
                }
            }
        }
    }
    , RadixLimit: function (n, number) {  //文本框中输入小数
        n = n.replace(/[^-\d.]/g, ""); //清除"数字"和"."以外的字符
        n = n.replace(/^\./g, ""); //验证第一个字符是数字而不是
        n = n.replace(/\.{2,}/g, "."); //只保留第一个. 清除多余的
        n = n.replace(".", "$#$").replace(/\./g, "").replace("$#$", ".");
        if (parseInt(number) >= 0) {
            var num = '';
            for (var i = 0; i < parseInt(number) ; i++) {
                num = num + '\\d';
            }
            num = '(' + num + ')';
            var reg = new RegExp("^(\\-)*(\\d+)\\." + num + ".*$");
            n = n.replace(reg, '$1$2.$3');
        }
        return n;
    }
    , GetIncreaseData: function (fields) {  //获取自增行内控件的数据
        var _this = this;
        var fieldsValue = [];
        if (fields && fields.length > 0) {
            if (_this.BaseData && _this.BaseData.showData && _this.BaseData.showData.length > 0) { //数据存在
                var fieldsData = [];
                //获取自增行内控件所有数据
                for (var i = 0; i < fields.length; i++) {
                    var fieldsdata = _this.FiltData(_this.BaseData.showData, 'Key', fields[i].id);  //获取控件的所有数据
                    if (fieldsdata && fieldsdata.length > 0) {
                        $.each(fieldsdata, function (j, y) {
                            fieldsData.push(y);
                        });
                    }
                }
                //控件数据行列转换
                if (fieldsData && fieldsData.length > 0) {
                    var num = fields.length;        //自增行内的控件数量
                    var count = fieldsData.length;  //自增行内控件值的数量
                    var p = count / num;            //自增行添加的行数                   
                    for (var j = 0; j < p; j++) {
                        var trFields = '{';
                        for (var k = 0; k < num; k++) {
                            trFields = trFields + '"' + fields[k].id + '":"';
                            //+ fieldsData[k * p + j].Value + '",';
                            if (fields[k].sfplugins == 'upload') {
                                trFields = trFields + fieldsData[k * p + j].Value + '|' + fieldsData[k * p + j].FileName;
                            }
                            else {
                                trFields = trFields + fieldsData[k * p + j].Value;
                            }
                            trFields = trFields + '",';
                        }
                        trFields = trFields.replace(/,$/g, '');
                        trFields = trFields + '}';
                        fieldsValue.push(JSON.parse(trFields));
                    }
                }
            }
        }
        return fieldsValue;
    }
    , PreventBack: function () {
        $("input,textarea").keydown(function (e) {
            var keyEvent;
            if (e.keyCode == 8) {
                var d = e.srcElement || e.target;
                if (d.tagName.toUpperCase() == "INPUT" || d.tagName.toUpperCase() == "TEXTAREA") {
                    keyEvent = d.readOnly || d.disabled;
                }
                else {
                    keyEvent = false;
                }
            }
            if (keyEvent) {
                e.preventDefault();
            }
        });
    }
    , GetIncreaseIds: function () {
        var ids = "";
        $("[sfplugins='increasetable']").each(function (i, x) {
            var id = $(x).attr("id");
            $("tr[id=" + id + "]>td").find("[sfplugins]").each(function (j, y) {
                var templateid = $(y).attr("id");
                ids = ids + templateid + ";";
            });
        });
        return ids;
    }
    , GetAjaxData: function (param) {  //ajax获取数据
        $.ajax({
            type: param.type,
            url: param.url,
            data: param.data,
            dataType: "json",
            success: function (result) {
                param.callback(result);
            }
        });
    }
    , DropBind: function (id, db, first, keyField, valField) {
        var ops = document.getElementById(id).options;
        ops.length = 0;
        //默认显示值
        if (first) {
            ops.add(new Option(first, -1));
        }
        $.each(db, function (idx, row) {
            //设置值
            var op = keyField || valField ?
                     new Option(row[keyField], row[valField]) :
                     new Option(row, row);
            ops.add(op);
        });
    }
    , SelectBind: function (id, db, keyField, valField, type) {
        var radio = $("#" + id);
        if (db != null && db.length > 0) {
            $.each(db, function (index, row) {
                var rd = keyField || valField ?
                    '<input type="' + type + '" name="' + id + '" text="' + row[keyField] + '" value="' + row[valField] + '"/>' + row[keyField] + '&nbsp' :
                    '<input type="' + type + '" name="' + id + '" text="' + row[keyField] + '" value="' + row + '"/>' + row + '&nbsp';
                radio.append(rd);
            });
        }
    }
    , FiltData: function (data, key, val) {
        var result = [];
        $.each(data, function (idx, d) {
            if (d[key] == val)
                result.push(d);
        });
        return result;
    }
    , FiltArray: function (array, objProperty, objValue) {   //过滤符合条件的类数组并返还数组
        var iReturn = new Array();
        $.grep(array, function (cur, i) {
            if (cur[objProperty] == objValue) {
                iReturn.push(cur)
            }
        }, false)
        return iReturn;
    }
    , AddMustSign: function (x) {
        var orgisrequire = $(x).attr("orgisrequire");
        var id = $(x).attr('id');
        if (orgisrequire == "true") {
            $(x).after("&nbsp;<span for='" + id + "' style='color:red;font-size:16px;'>*</span>");
        }
    }
    , CheckStyle: function (x) {
        var browserVersion = navigator.appVersion;
        if (browserVersion.indexOf("MSIE 8") == -1 && browserVersion.indexOf("MSIE 7") == -1) {
            var id = $(x).attr("id");
            var items = $("span input[name=" + id + "]");
            var orgradiotype = $(x).attr("orgradiotype");
            $(x).empty();
            $(items).each(function (i, y) {
                var eleType = $(y).attr("type");
                var val = $(y).attr("value");
                var txt = $(y).attr("text");
                if (typeof (txt) == "undefined") {
                    txt = val;
                }
                $(y).appendTo($(x))
                    .wrap('<div class="' + eleType + ' div-space-0"><label></label></div>')
                    .after('<span class="text">' + txt + '</span>');
            });
            if (orgradiotype == "Vertical") {  //竖排排列
                $(x).find('.div-space-0').after("<br>");
            }
        }
    }
    , AgainStyle: function (x) {   //重新定义样式（单选，复选）
        var html = "";
        var orgradiotype = $(x).attr("orgradiotype");
        $(x).find('input[type="radio"],input[type="checkbox"]').each(function (i, y) {
            html = html + '<label style="font-weight: normal;">' + $(y).prop('outerHTML') + $(y).attr('value') + '&nbsp;</label>';
            if (orgradiotype == "Vertical") {
                html = html + "<br/>";
            }
        });
        $(x).empty().append(html);

        return;

        //var id = $(x).attr("id");
        //if (navigator.userAgent.indexOf("isoffice") > -1) {
        //    $("input[name=" + id + "]").css({
        //        'opacity': '1',
        //        'position': 'relative',
        //        'left': '0',
        //        'z-index': '12',
        //        'width': '18px',
        //        'height': '18px',
        //        'cursor': 'pointer'
        //    });
        //}
        //else {
        //    FormUI.CheckStyle(x);
        //}
    }
    , ProAutonum: function (x) {   //生成编号
        var id = $(x).attr("id") + 'number';
        var strVal = '';
        var begin = 0;
        var strBegin;
        var myDate = new Date();
        var year = myDate.getFullYear(),                             //年
            month = FormUI.PrefixInteger(myDate.getMonth() + 1, 2),  //月
            day = FormUI.PrefixInteger(myDate.getDate(), 2),         //日
            hours = FormUI.PrefixInteger(myDate.getHours(), 2),      //时
            minutes = FormUI.PrefixInteger(myDate.getMinutes(), 2),  //分
            seconds = FormUI.PrefixInteger(myDate.getSeconds(), 2),  //秒
            week = myDate.getDay();                                  //星期 
        strVal += $(x).attr("orgprefix");
        if (!$(x).attr("orgdatetype")) {
            //获取当期日期
            strVal += year + "-" + month + "-" + day + " " + hours + ":" + minutes + ":" + seconds;
        } else {
            strVal += $(x).attr("orgdatetype")
                          .replace("yyyy", year)
                          .replace("MM", month)
                          .replace("dd", day)
                          .replace("HH", hours)
                          .replace("mm", minutes)
                          .replace("ss", seconds)
                          .replace("DD", "星期" + new Array("日", "一", "二", "三", "四", "五", "六")[week]);
        }
        strVal += $(x).attr("orgseparator");
        var fieldName = $(x).attr("id");   //自动编号控件ID
        var figures = $(x).attr("orgnumfigures");   //流水号位数
        if ($.isNumeric(figures) && figures > 0) {
            if ($(x).attr("orgnumfigures")) {   //需要流出水编号
                begin = 0;
                if ($(x).attr("orgnumbegin")) {
                    begin = $(x).attr("orgnumbegin");
                }
                if ($(x).closest("tr[sfplugins=increasetable]").length == 0 && FormUI.PageType == 1) {   //非自增列自动编号控件需取数据库中流水号
                    var param = {
                        url: FormUI.curRootPath + '/WfFlowSend/GetBeginNum?r=' + Math.random()
                      , type: 'Get'
                      , data: { 'FieldName': fieldName, 'FormVID': $(x).attr("orgFormVId"), "PartNum": encodeURI(strVal) }
                    }
                    param.callback = function (result) {
                        if (result.flag == 1) {
                            begin = result.data;
                        }
                        else {
                            if ($(x).attr("orgnumbegin")) {
                                begin = $(x).attr("orgnumbegin");
                            }
                        }
                        strBegin = FormUI.PrefixInteger(begin, parseInt(figures));  //根据编号位数 左边补0
                        strVal += strBegin;
                        FormUI.ReplaceAuto(x, id, strVal, begin);
                    }
                    FormUI.GetAjaxData(param);
                }
                else {
                    if ($(x).attr("orgnumbegin")) {
                        begin = $(x).attr("orgnumbegin");
                    }
                    strBegin = FormUI.PrefixInteger(begin, parseInt(figures));  //根据编号位数 左边补0
                    strVal += strBegin;
                    FormUI.ReplaceAuto(x, id, strVal, begin);
                }

            }
            else {
                FormUI.ReplaceAuto(x, id, strVal, begin);
            }
        }
        else {
            FormUI.ReplaceAuto(x, id, strVal, begin);
        }
    }
    , ReplaceAuto: function (x, id, strVal, begin) {       //自增编号
        var oNode = $("<lable>", { 'name': id, 'id': id });
        oNode.html(strVal);
        $(x).after(oNode);
        $(x).attr("begin", begin);
        $(x).val(strVal);
        $(x).hide();
    }
    , GetCurrenData: function () {
        var date = new Date();
        var seperator1 = "-";
        var seperator2 = ":";
        var year = date.getFullYear();
        var month = date.getMonth() + 1;
        var strDate = date.getDate();
        var hours = date.getHours();
        var minutes = date.getMinutes();
        if (month >= 1 && month <= 9) {
            month = "0" + month;
        }
        if (strDate >= 0 && strDate <= 9) {
            strDate = "0" + strDate;
        }
        if (hours >= 0 && hours <= 9) {
            hours = "0" + hours;
        }
        if (minutes >= 0 && minutes <= 9) {
            minutes = "0" + minutes;
        }


        var currentdate = year + seperator1 + month + seperator1 + strDate
                + " " + hours + seperator2 + minutes;
        return currentdate;
    }
    , Calculate: function (x) {
        var _this = this;
        try {
            var calObj = $("#" + x);//隐藏域 用于传值到后台存储
            if (!calObj) return;//控件不存在退出
            var relObj = $("#" + calObj.attr("relid"));//span区域里存储了运算公式等属性
            var val = calObj.attr("orgformulaval");
            var type = calObj.attr("orgresultformat");  //计算结果格式
            var format = calObj.attr("orgdataformat");
            var reg = new RegExp("[+-//*()]"); //匹配运算符号
            var opt = val.split(reg);
            var result;
            for (var i = 0; i < opt.length; i++) {

                if (opt[i] == "") {
                    continue;
                }

                if (!$("#" + opt[i])[0]) {//假如其中一个控件无法找到 则跳出
                    if (!FormUI.BaseData || !FormUI.BaseData.FieldPower || FormUI.BaseData.FieldPower.length == 0) {
                        return;
                    }
                    else {
                        var optdata = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", opt[i]);  //获取控件的权限
                        if (optdata.length == 0 || optdata[0].IsVisible) {
                            layer.msg('计算框中的一个项目找不到！！！');
                            //alert("计算框中的一个项目找不到！！！");
                        }
                        return;
                    }
                }

                var sfplugins = $("#" + opt[i]).attr("sfplugins");
                var itemtype = $("#" + opt[i]).attr("orgtype");
                if (sfplugins == "text") {   //对数值进行验证
                    if (itemtype && itemtype != '') {
                        var regitem = "";
                        switch (itemtype) {
                            case "int":
                                regitem = FormUI.regint;
                                break;
                            case "zint":
                                regitem = FormUI.regzint;
                                break;
                            case "fint":
                                regitem = FormUI.regfint;
                                break;
                            case "float":
                                regitem = FormUI.regfloat;
                                break;
                            case "percent":
                                regitem = FormUI.regpercent;
                                break;
                        }
                        if (!regitem.test($("#" + opt[i]).val())) {
                            calObj.val("").change();
                            relObj.text("");
                            return false;
                        }
                    }
                }
                if ($("#" + opt[i]).val() == "") {//假如控件无输入 则不计算
                    calObj.val("").change();
                    relObj.text("");
                    return;
                }
                if (type == 'date') {
                    var orgdatetype = $("#" + opt[i]).attr("orgdatetype");
                    var optval = $("#" + opt[i]).val();
                    var myDate = new Date();
                    if (orgdatetype == "yyyy") {
                        optval = optval + "/" + (myDate.getMonth() + 1) + "/" + myDate.getDay();
                    }
                    else if (orgdatetype == "MM") {
                        optval = myDate.getFullYear() + "/" + optval + "/" + myDate.getDay();
                    }
                    else if (orgdatetype == "dd") {
                        optval = myDate.getFullYear() + "/" + (myDate.getMonth() + 1) + "/" + optval;
                    }
                    else if (orgdatetype == "HH:mm") {
                        optval = myDate.getFullYear() + "/" + (myDate.getMonth() + 1) + "/" + myDate.getDay() + " " + optval;
                    }
                    else if (orgdatetype == "yyyy-MM-dd DD") {
                        optval = optval.split(" ")[0];
                    }
                    val = val.replace(opt[i], new Date(Date.parse(optval.replace(/-/g, "/").replace(/年/g, "/").replace(/月/g, "/").replace(/日/g, "/"))).getTime());
                }
                else {
                    var percent = "";
                    if ($("#" + opt[i]).attr("orgtype") == "percent" && FormUI.regpercent.test($("#" + opt[i]).val())) {
                        var percent = "*0.01";
                    }
                    val = val.replace(opt[i], $("#" + opt[i]).val() + percent).replace(/%/g, '');
                }
            }
            if (type == 'int' || type == 'float') {//整数 小数
                result = eval(val.replace(/,/g, ''));
                if (!isFinite(result)) {
                    layer.alert('计算公式中存在除数为0，不能计算出结果！', { icon: 2 });
                    calObj.val("").change();
                    relObj.text("");
                    return;
                }
                var strResult = result.toString();
                if (type == 'int') {//int
                    result = Math.round(result);   //四舍五入为整数
                    if (format == '#,###') {
                        strResult = result.toString().replace(/(\d{1,3})(?=(?:\d{3})+(?!\d))/g, '$1,');//每3位数字中间加个逗号
                        if (strResult.length % 4 == 0) {//特殊处理一下 4位时  ",123"  8位时  ",534,224"
                            strResult = strResult.substr(1, strResult.length - 1);
                        }
                    }
                    else if (format == 'money' || format == '人民币大写') {
                        strResult = FormUI.NumToCny(result);
                    }
                    else {
                        strResult = result;
                    }
                } else if (type == 'float') {//float
                    if (format == "round" || format == "四舍五入") {
                        result = Math.round(result);
                    } else if (format == 'ceil' || format == '结果加1') {//向上取整,有小数就整数部分加1
                        result = Math.ceil(result);
                    } else if (format == 'decimal' || format == '保留小数') {
                        var dotnum = $("#" + x).attr("orgdotnumber");
                        result = Number(result).toFixed(Number(dotnum));
                    }
                    else if (format == 'money' || format == '人民币大写') {
                        result = FormUI.NumToCny(result);
                    }
                    strResult = result.toString();
                    var temp = strResult.split('.');
                    temp[0] = temp[0].replace(/(\d{1,3})(?=(?:\d{3})+(?!\d))/g, '$1,');
                    if (strResult.length % 4 == 0) {
                        strResult = strResult.substr(1, strResult.length - 1);
                    }
                    strResult = temp.join('.');
                }
                calObj.val(strResult).change();
                relObj.text(strResult);
            }
            else if (type == 'percent') {// 30% + 20%
                val = '(' + val + ')*100';
                if (val) {
                    result = eval((val));
                    if (!isFinite(result)) {  //检查参数是否无穷大（即除数是否为0）
                        layer.alert('计算公式中存在除数为0，不能计算出结果！', { icon: 2 });
                        calObj.val("").change();
                        relObj.text("");
                        return;
                    }
                    if (format == 'round' || format == "四舍五入") {
                        result = Math.round(result);
                    } else if (format == 'ceil' || format == '结果加1') {//向上取整,有小数就整数部分加1
                        result = Math.ceil(result);
                    } else if (format == 'decimal' || format == '保留小数') {
                        var dotnum = $("#" + x).attr("orgdotnumber");
                        result = Number(result).toFixed(Number(dotnum));
                    }
                    calObj.val(result + '%').change();
                    relObj.text(result + '%');
                }
            }
            else if (type == 'date') {
                var day = 24 * 60 * 60 * 1000;
                var time = 60 * 60 * 1000;
                var minute = 60 * 1000;
                result = eval(val);
                var r = FormUI.ConverDate(result, format);   //计算时间差
                if (format == "D") {
                    r = (parseInt(r.replace(/天/, '')) + 1) + '天';
                }
                calObj.val(r).change();
                relObj.text(r);
            }
        } catch (e) {
            return false;
        }
    }
    , NumToCny: function (n) {   //人民币转换成大写
        var fraction = ['角', '分'];
        var digit = ['零', '壹', '贰', '叁', '肆', '伍', '陆', '柒', '捌', '玖'];
        var unit = [['元', '万', '亿'], ['', '拾', '佰', '仟']];
        var head = n < 0 ? '欠' : '';
        n = Math.abs(n).toFixed(2);

        var s = '';
        var x = n.toString()
        if (x.indexOf('.') >= 0) {
            y = x.split('.')[1];  //小数部分
            if (y != "0" && y != "00") {
                if (y.length > 0) {
                    s += digit[y.substr(0, 1)] + fraction[0];
                }
                if (y.length > 1) {
                    s += digit[y.substr(1, 1)] + fraction[1];
                }
            }
        }
        //for (var i = 0; i < fraction.length; i++) {
        //    s += (digit[Math.floor(n * 10 * Math.pow(10, i)) % 10] + fraction[i]).replace(/零./, '');
        //}
        s = s || '整';
        n = Math.floor(n);

        for (var i = 0; i < unit[0].length && n > 0; i++) {
            var p = '';
            for (var j = 0; j < unit[1].length && n > 0; j++) {
                p = digit[n % 10] + unit[1][j] + p;
                n = Math.floor(n / 10);
            }
            s = p.replace(/(零.)*零$/, '').replace(/^$/, '零') + unit[0][i] + s;
        }
        var result = s.replace(/(零.)*零元/, '元').replace(/(零.)+/g, '零').replace(/^整$/, '零元整');
        var reg1 = /元零$/, reg2 = /角零$/;
        if (reg1.test(result) || reg2.test(result)) {
            result = result + '分';
        }
        return head + result;
    }
    , ConverDate: function (val, format) {  //将时间结果转换成需要的格式
        var objInterval = { 'D'/*天*/: 1000 * 60 * 60 * 24, 'H'/*小时*/: 1000 * 60 * 60, 'M'/*分钟*/: 1000 * 60, 'S'/*秒*/: 1000, 'T'/*毫秒*/: 1 };
        var temp = format.toUpperCase().split("-");
        var result = "";
        for (var i = 0; i < temp.length; i++) {
            var Interval = 0;
            if (i == temp.length - 1) {
                Interval = Math.round(parseFloat(val / eval('objInterval.' + temp[i])));   //四舍五入（最后一项）
            }
            else {
                Interval = Math.floor(parseFloat(val / eval('objInterval.' + temp[i])));   //向下取整
            }
            result = result + Interval + temp[i].replace(/D/, '天').replace(/H/, '小时').replace(/M/, '分钟').replace(/S/, '秒');
            val = val - parseInt(eval('objInterval.' + temp[i]) * Interval);
        }
        return result;
    }
    , AddImage: function (e) {
        var id = $(e).attr("id");
        var filePath = $(e).val();
        var fileName = "";
        if (filePath.length > 0) {
            var ldot = filePath.lastIndexOf(".");
            var type = filePath.substr(ldot).toString().toLocaleLowerCase();
            var ext = ['.gif', '.jpg', '.jpeg', '.png'];
            if (ext.indexOf(type) == -1) {
                $(e).val("");
                layer.alert('图片限于png,gif,jpeg,jpg格式！', { icon: 2 });
                //alert("图片限于png,gif,jpeg,jpg格式！");
                return false;
            }
            else {

                var start = filePath.lastIndexOf("\\");

                if (start > 0) {
                    fileName = filePath.substr(start + 1);
                }
                else {
                    fileName = filePath;
                }
                if (FormUI.PageType == 0) {
                    return;
                }
                FormUI.UploadImage(e, fileName);
            }
        }
    }
    , UploadImage: function (e, fileName) {
        var id = $(e).attr("id");
        $('#formShow').ajaxSubmit({
            url: FormUI.curRootPath + '/CsFile/Upload/' + id,
            data: { directoryId: 111100/*工作流文件码*/, fileCustomId: FormUI.FlowApplyID/*流程编号*/, fileUploadId: id/*控件Id*/ },
            type: 'Post',
            success: function (r) {
                if (r.flag == 1) {
                    var src = FormUI.curRootPath + "/CsFile/Download/" + r.data.id;
                    var orgtype = $("#" + id + "imageName").attr("orgtype");
                    var width, height, orgwidth, orgheight, orgwidthtype;
                    if (orgtype == "Preinstall") {   //预设图片大小   
                        var s;
                        orgwidthtype = $("#" + id + "imageName").attr("orgwidthtype");
                        if (orgwidthtype == "pixel") {
                            s = "px";
                        }
                        else {
                            s = "%";
                        }
                        orgwidth = $("#" + id + "imageName").attr("orgwidth");
                        orgheight = $("#" + id + "imageName").attr("orgheight");
                        width = orgwidth + s;
                        height = orgheight + s;
                    }

                    $("#" + id + "preview").html("<img id='" + id + "imageName' orgtype='" + orgtype + "'/>");
                    if (orgtype == "Preinstall") {   //预设图片大小
                        $("#" + id + "imageName").css("width", width);
                        $("#" + id + "imageName").css("height", height);

                        $("#" + id + "imageName").attr("orgwidth", orgwidth);
                        $("#" + id + "imageName").attr("orgheight", orgheight);
                        $("#" + id + "imageName").attr("orgwidthtype", orgwidthtype);

                    }
                    $("#" + id + "imageName").attr("src", src);
                    $("#" + id + "imagePath").val(r.data.id);
                }
                else {
                    layer.msg('图片上传失败！');
                    return;
                }
            }
        });

        //$("#formShow").ajaxSubmit({
        //    //url: "FormAction.ashx?action=saveuploadfile&sfType=2&UploadFileID=" + id + "&UploadFileName=" + encodeURI(fileName),
        //    url:FormUI.curRootPath+ '/WfFormDefine/SaveUpload',
        //    type: "POST",
        //    data: { 'sfType': 2, 'UploadFileID': id, UploadFileName: encodeURI(fileName) },
        //    success: function (r) {
        //        if (r.flag == 1) {
        //            var src = FormUI.curRootPath + "/PublicFile/FormAttachment/" + r.data;
        //            var id = $(e).attr("id");

        //            var orgtype = $("#" + id + "imageName").attr("orgtype");
        //            var width, height, orgwidth, orgheight, orgwidthtype;

        //            if (orgtype == "Preinstall") {   //预设图片大小   
        //                var s;
        //                orgwidthtype = $("#" + id + "imageName").attr("orgwidthtype");
        //                if (orgwidthtype == "pixel") {
        //                    s = "px";
        //                }
        //                else {
        //                    s = "%";
        //                }
        //                orgwidth = $("#" + id + "imageName").attr("orgwidth");
        //                orgheight = $("#" + id + "imageName").attr("orgheight");


        //                width = orgwidth + s;
        //                height = orgheight + s;
        //            }

        //            $("#" + id + "preview").html("<img id='" + id + "imageName' orgtype='" + orgtype + "'/>");
        //            if (orgtype == "Preinstall") {   //预设图片大小
        //                $("#" + id + "imageName").css("width", width);
        //                $("#" + id + "imageName").css("height", height);

        //                $("#" + id + "imageName").attr("orgwidth", orgwidth);
        //                $("#" + id + "imageName").attr("orgheight", orgheight);
        //                $("#" + id + "imageName").attr("orgwidthtype", orgwidthtype);

        //            }
        //            $("#" + id + "imageName").attr("src", src);
        //            $("#" + id + "imagePath").val(r.data);
        //        }
        //        else {
        //            layer.msg('图片上传失败！');
        //            //alert("图片上传失败！");
        //            return;
        //        }

        //    },
        //    error: function (d) {
        //    }
        //});
    }
    , Upload: function (x) {
        var id = $(x).attr("id")
           , p = $("#" + id).val()
           , fileName = "", name = "";
        var filePath = $("#" + id).val();
        if (filePath.lastIndexOf(".") > 0) {
            var start = filePath.lastIndexOf("\\") + 1;
            if (start > 0) {
                fileName = filePath.substr(start);
            }
            else {
                fileName = filePath;
            }
            name = fileName.substring(0, fileName.lastIndexOf("."));
        }

        $("#formShow").ajaxSubmit({
            type: 'Post',
            url: FormUI.curRootPath + '/CsFile/Upload',
            data: { directoryId: 111100, fileCustomId: FormUI.FlowApplyID },
            success: function (result) {
                if (result.flag == 1) {
                    var fileIds = $("#" + id + "filePath").val();
                    fileIds += result.data.id + ",";
                    $("#" + id + "filePath").val(fileIds);
                    FormUI.FileList(id, fileName, result.data.id);
                }
                else {
                    layer.msg('上传失败！');
                    return false;
                }
            }
        });
    }
    , FileList: function (id, fileName, fileId) {
        var fileListID = id + "fileList";
        if ($("#" + fileListID).length == 0) {
            var tbWidth = "", tbWidthFile = "";
            tbWidth = $("#" + id).parent().css("width");   //宽
            $("#div" + id).after("<div id='" + fileListID + "' style='word-break:break-all;'><table style='width:" + tbWidth + ";min-width: 250px;max-width: 500px;'></table></div>");
        }
        var fileHref = "", fileHtml = "";
        if (FormUI.PageType == 0) {
            url = "javescript:void(0);";
        }
        var URL = FormUI.curRootPath + '/CsFile/Download/' + fileId;
        fileHtml += "<tr>";
        fileHtml += "<td><input id='" + id + "uploadFileName' type='hidden' value='" + fileId + "' />" + fileName + "</td>";
        fileHtml += '<td style="text-align:center;width:60px;"><a href="' + URL + '">查看</a></td>';    //查看
        if (FormUI.FlowStatus != 0) {
            var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的权限
            if (data.length > 0 && data[0].IsVisible == 1 && data[0].IsReadOnly == 0) {
                fileHtml += "<td style='text-align:center;width:60px;'><a href='#' alt='" + fileId + "' fileid='" + id + "' onclick='FormUI.FileDelete(this)'>删除</a></td>";  //删除
            }
        }
        fileHtml += "</tr>";
        $("#" + fileListID + " table").append(fileHtml);   //加载Html   
    }
    , FileUpload: function (x) {
        var id = $(x).attr("id"), fileName = "";
        var filePath = $("#" + id).val();
        if (filePath.lastIndexOf(".") > 0) {
            var start = filePath.lastIndexOf("\\") + 1;
            if (start > 0) {
                fileName = filePath.substr(start);  //文件名称
            }
            else {
                fileName = filePath;
            }
        }
        $("#formShow").ajaxSubmit({
            type: 'Post',
            url: FormUI.curRootPath + '/CsFile/Upload/' + id,
            data: {
                directoryId: 111100/*工作流文件码*/
              , fileCustomId: FormUI.FlowApplyID/*流程编号*/
              , fileUploadId: id/*控件Id*/
              , fileName: fileName/*文件名称*/
            },
            success: function (result) {
                if (result.flag == 1) {
                    var fileIds = $("#" + id + "filePath").val();
                    fileIds += result.data.id + ",";
                    $("#" + id + "filePath").val(fileIds);
                    FormUI.FileList(id, fileName, result.data.id);
                }
                else {
                    layer.msg('上传失败！');
                    return false;
                }
            }
        });
    }
    , FileDelete: function (e) {
        var filePath = $(e).attr("alt");       //被删除的文件路径
        var id = $(e).attr("fileid");          //上传控件的ID
        $(e).parent().parent().remove();       //移除被删除的文件

        var filePathList = "";
        $("#" + id + "fileList tr td #" + id + "uploadFileName").each(function (i, x) {
            filePathList += $(x).val() + ",";
        });
        $("#" + id + "filePath").val(filePathList);  //重新赋值

        $("#" + id + "fileName").val("");           //清空上传控件
        $("#" + id).val("");

        //删除文件夹中的文件--edit
        $.ajax({
            type: 'Post',
            data: { filePath: filePath },
            url: FormUI.curRootPath + "/WfFormDefine/DeleteUpload",
            success: function (result) {
                if (result.flag = 1) {
                    layer.msg(result.msg);
                }
                else {
                    layer.msg(result.msg);
                    return;
                }
            }
        });
    }
    , GetUploadFilePath: function (x) {
        FormUI.FileUpload(x);
        return false;
        //var id = $(x).attr("id");
        //var p = $("#" + id).val();
        ////$("#" + id + "fileName").val(p);
        //if (p == null || p == "") {
        //    return false;
        //}
        //if (p.lastIndexOf(".") > 0) {
        //    FormUI.AddFiles(x);
        //}
        //else {
        //    layer.alert('文件路径有误，请联系管理员！', { icon: 2 });
        //    //alert("文件路径有误，请联系管理员！");
        //    return;
        //}
    }
    , AddFiles: function (e) {  //上传文件到文件夹
        var fileName = "", name = "";
        var id = $(e).attr("id");
        var filePath = $("#" + id).val();
        if (filePath.lastIndexOf(".") > 0) {
            var start = filePath.lastIndexOf("\\") + 1;
            if (start > 0) {
                fileName = filePath.substr(start);
            }
            else {
                fileName = filePath;
            }
            name = fileName.substring(0, fileName.lastIndexOf("."));
        }
        if (name == "") {
            layer.alert('文件名不能为空！', { icon: 2 });
            //alert("文件名不能为空！");
            $("#" + id).val("");
            return false;
        }
        if (name.match(/&/) != null) {
            layer.alert('上传文件名不能包含 & 字符！！！', { icon: 2 });
            //alert("上传文件名不能包含 & 字符！！！");
            $("#" + id).val("");
            return false;
        }
        else {
            var id = $(e).attr("id");

            if (FormUI.PageType == 0) {
                var filenamelist = $("#" + id + "filePath").val();
                if (filenamelist) {
                    filenamelist = filenamelist + "," + fileName;
                }
                else {
                    filenamelist = fileName;
                }
                $("#" + id + "filePath").val(filenamelist);
                FormUI.AddFileListHtml(id, filenamelist);    //添加附件列表
            }
            else {
                $("#formShow").ajaxSubmit({
                    //url:FormUI.curRootPath+ "/WfFormDefine/SaveUpload?sfType=1&UploadFileID=" + $(e).attr("id") + "&UploadFileName=" + encodeURI(fileName),
                    url: FormUI.curRootPath + "/CsFile/Upload",
                    type: "POST",
                    data: { directoryId: 111100 },
                    success: function (r) {
                        if (r.flag == 1) {
                            var filePath = $("#" + id + "filePath").val();
                            filePath += r.data + ",";
                            $("#" + id + "filePath").val(filePath);
                            FormUI.AddFileListHtml(id, r.data);    //添加附件列表
                        }
                        else {
                            layer.msg('上传失败！');
                            //alert("上传失败!");
                            return false;
                        }
                    }
                });
            }
        }
    }
    , AddFileListHtml: function (id, filePath) {  //加载上传文件列表
        var fileListID = id + "fileList";
        if ($("#" + fileListID).length == 0) {
            var tbWidth = "", tbWidthFile = "";
            tbWidth = $("#" + id).parent().css("width");   //宽
            $("#div" + id).after("<div id='" + fileListID + "' style='word-break:break-all;'><table style='width:" + tbWidth + ";min-width: 250px;max-width: 500px;'></table></div>");
        }
        var fileName = "", fileHref = "", fileHtml = "";
        var start = filePath.lastIndexOf("/");
        if (FormUI.PageType == 0) {
            start = filePath.lastIndexOf(",");
        }
        var len = filePath.length;
        if (start > 0) {
            start = start + 1;
            fileName = filePath.substr(start, len - start);  //文件名称
        }
        else {
            fileName = filePath;
        }
        var filetype = fileName.substr(fileName.lastIndexOf("."));
        var strpath = filePath.substr(0, filePath.lastIndexOf("."));

        if (FormUI.PageType == 0) {
            url = "javescript:void(0);";
        }
        fileHtml += "<tr>";
        fileHtml += "<td><input id='" + id + "uploadFileName' type='hidden' value='" + filePath + "' />" + fileName + "</td>";
        fileHtml += '<td style="text-align:center;width:60px;"><a href="#" onclick="FormUI.LookFile(\'' + filetype + '\',\'' + strpath + '\',\'' + fileName + '\')">查看</a></td>';    //查看
        if (FormUI.FlowStatus != 0) {
            var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的权限
            if (data.length > 0 && data[0].IsVisible == 1 && data[0].IsReadOnly == 0) {
                fileHtml += "<td style='text-align:center;width:60px;'><a href='#' alt='" + strpath + "' fileid='" + id + "' onclick='FormUI.DeleteUploadFile(this)'>删除</a></td>";  //删除
            }
        }
        fileHtml += "</tr>";
        $("#" + fileListID + " table").append(fileHtml);   //加载Html        
    }
    , LookFile: function (FileType, FilePath, FileName) {
        filetype = encodeURIComponent(FileType);
        strpath = encodeURIComponent(FilePath);
        strname = encodeURIComponent(FileName);
        $.ajax({
            type: "Get",
            url: FormUI.curRootPath + "/WfFormDefine/GetFileIsExists", //查看文件是否存在
            data: {
                filetype: filetype,
                strpath: strpath,
                strname: strname
            },
            success: function (data) {
                if (data.flag == 1) {   //存在
                    if (navigator.userAgent.indexOf("isoffice") > -1) {   //判断是否是在手机端
                        //FormUI.LookFileByApi(FileType, FilePath, FileName);
                        window.location.href = FormUI.curRootPath + "/api/WorkFlowNew/Download?FileType=" + FileType + "&FilePath=" + FilePath + "&FileName=" + FileName;
                    }
                    else {
                        window.location.href = FormUI.curRootPath + "/WfFormDefine/FileUpload?filetype=" + filetype + "&strpath=" + strpath + "&strname=" + strname;  //下载文件
                    }
                }
                else {
                    layer.msg(data.msg);
                    //alert(data.msg);
                    return;
                }
            }
        });

    }
    , LookFileByApi: function (FileType, FilePath, FileName) {
        $.ajax({
            type: 'Get',
            url: FormUI.curRootPath + '/api/WorkFlowNew/Download',  //api地址
            data: { FileType: FileType, FilePath: FilePath, FileName: FileName },
            success: function (result) {

            }
        });

    }
    , DeleteUploadFile: function (e) {
        var filePath = $(e).attr("alt");       //被删除的文件路径
        var id = $(e).attr("fileid");          //上传控件的ID
        $(e).parent().parent().remove();       //移除被删除的文件

        var filePathList = "";
        $("#" + id + "fileList tr td #" + id + "uploadFileName").each(function (i, x) {
            filePathList += $(x).val() + ",";
        });
        $("#" + id + "filePath").val(filePathList);

        $("#" + id + "fileName").val("");
        $("#" + id).val("");

        //删除文件夹中的文件--edit
        $.ajax({
            type: 'Post',
            data: { filePath: filePath },
            //url: 'FormAction.ashx?action=deluploadfile',
            url: FormUI.curRootPath + "/WfFormDefine/DeleteUpload",
            success: function (result) {
                if (result.flag = 1) {
                    layer.msg(result.msg);
                    //alert(result.msg);
                }
                else {
                    layer.msg(result.msg);
                    //alert(result.msg);
                    return;
                }
            }
        });
    }
    , InitTable: function (objTb, cols, dataSet) {
        if (cols && dataSet) {
            var oNode = $("<div>", { 'name': objTb.attr("id"), 'id': objTb.attr("id") });
            oNode.append('<table id="example" class="display cell-border" cellspacing="0" width="100%"></table>');
            oNode.height(objTb.height());
            oNode.width(objTb.width());
            oNode.css("overflow", "auto");
            objTb.parent().append(oNode);
            $('#example').dataTable({
                //"scrollX": true,
                "bFilter": false,
                "bLengthChange": false,
                "bInfo": false,
                "bSort": false,
                "iDisplayLength": 5,
                //"ajax": "data/objects.txt",
                "data": dataSet,
                "columns": cols
            });
            objTb.remove();
        }
    }
    , AddTableRow: function (id) {  //自增表格增加行
        var x = $("#" + id);
        var s = $(x).clone().prop("outerHTML");
        var itemcount = 0;
        var id = $(x).attr("id");

        $(".tablestyle").on("click", "tr[id=" + id + "] button[type=button].increasetableadd", function (ids, y) {
            if (typeof (AddIncreaseRowFunction) != 'undefined' && $.isFunction(AddIncreaseRowFunction)) {
                if (!AddIncreaseRowFunction())   //实时验证
                {
                    return;
                }
            }
            if (itemcount == 0) {
                var count = 0;
                var regName = id + "Sub";
                $("tr[id^='" + id + "Sub']").each(function (i, e) {
                    var n = $(e).attr("id").replace(regName, '')
                    if (parseInt(n) > parseInt(count)) {
                        count = n;
                    }
                });
                itemcount = count;
            }
            itemcount = parseInt(itemcount) + 1;

            $("#" + id + ">td").find("[sfplugins]").each(function (i, e) {
                var itemid = $(e).attr("id");             //获取控件ID
                var sfplugins = $(e).attr("sfplugins");   //控件类型
                $(e).attr("id", itemid + "Item" + itemcount);
                $(e).attr("name", itemid + "Item" + itemcount);
                $(e).attr("sfplugins-increasetable", id + "Sub" + itemcount);
                $(e).attr("sfplugins-item", sfplugins);
                $(e).removeAttr("sfplugins");


                var newitemid = $(e).attr("id");

                if (sfplugins == "radios")         //单选控件
                {
                    $("#" + newitemid + " input[name=" + itemid + "]").attr("name", newitemid);
                }
                else if (sfplugins == "checkboxs")   //复选控件
                {
                    $("#" + newitemid + ">input[name=" + itemid + "]").attr("name", newitemid);
                }
                else if (sfplugins == "upload")   //上传控件
                {
                    $("#" + itemid + "Item" + itemcount).attr("type", "hidden");
                    $("#div" + itemid).attr("id", "div" + newitemid);
                    $("#" + itemid + "filePath").attr("name", newitemid + "filePath");
                    $("#" + itemid + "filePath").attr("id", newitemid + "filePath");
                    $("#btnSelect" + itemid).attr("name", "btnSelect" + newitemid);
                    $("#btnSelect" + itemid).attr("id", "btnSelect" + newitemid);
                    $("#" + itemid + "fileList").attr("id", newitemid + "fileList");
                    $("#" + newitemid + "fileList").find("[fileid=" + itemid + "]").each(function (i, x) {
                        $(e).attr("fileid", itemid);
                    });
                }
                else if (sfplugins == "image")   //上传控件
                {
                    $("#" + itemid + "preview").attr("id", newitemid + "imageName");
                    $("#" + itemid + "imageName").attr("id", newitemid + "imageName");
                    $("#" + itemid + "imagePath").attr("id", newitemid + "imagePath");
                    $("#" + itemid + "imagePath").attr("name", newitemid + "imagePath");
                }
                else if (sfplugins == "calculation") {
                    $("#" + itemid + "calcula").attr("id", newitemid + "calcula");
                }
                else if (sfplugins == "macros") {
                    if ($(e).attr("orgtype") == 2) {
                        $("#" + itemid + "UserName").attr("name", newitemid + "UserName");
                        $("#" + itemid + "UserName").attr("id", newitemid + "UserName");
                        $("#" + itemid + "UserCode").attr("name", newitemid + "UserCode");
                        $("#" + itemid + "UserCode").attr("id", newitemid + "UserCode");
                        $("#" + itemid + "selectButton").attr("onclick", "javescript:FormUI.UserDialog('" + newitemid + "')");
                        $("#" + itemid + "selectButton").attr("id", newitemid + "selectButton");
                    }
                }
                else if (sfplugins == "htmlarea") {
                    $("#" + itemid + "editor").attr("id", newitemid + "editor");
                    $("#" + newitemid).attr("relid", newitemid + "editor");
                }
                //else if (sfplugins == "autonumber") {  //自动编号框
                //}
            });
            var tr = $(this).closest("tr");
            $(tr).find(".increasetableadd").removeClass().addClass("increasetabledel").val("-");
            $(tr).attr("id", id + "Sub" + itemcount);
            $(tr).attr("name", id + "Sub" + itemcount);
            //$("#" + id + "Sub" + itemcount).find("input").attr('readonly', true);
            $(tr).removeAttr("sfplugins");
            FormUI.RbgTableRow(id);   //表格行的背景
            $("tr[id^=" + id + "]:last").after(s);
            $("#" + id + ">td>input[orgisrequire!=true][type!=button]").val("");
            $(".increasetabledel span").removeClass("glyphicon-plus").addClass("glyphicon-minus");
            $("#" + id + ">td").find("[sfplugins]").each(function (k, z) {
                $(z).attr("increasecontrol", "1");
            });
            FormUI.initIncreaseTrControl(id, itemcount, 1);
            FormUI.GetStatis();                 //计算统计控件值
        });
    }
    , initIncreaseTrControl: function (id, itemcount, type) {   //初始化自增行模板行控件
        var _this = this;
        $("#" + id + ">td").find("[sfplugins]").each(function (i, x) {
            var itemid = $(x).attr("id");             //获取控件ID
            var sfplugins = $(x).attr("sfplugins");   //控件类型
            var increasecontrol = $(x).attr("increasecontrol");
            if (increasecontrol == 1) {
                switch (sfplugins) {
                    case "text":           //文本框
                        _this.setText(x, 1);
                        break;
                    case "select":         //下拉框
                        _this.setSelects(x, 1);
                        break;
                    case "textarea":       //文本区
                        _this.setTextarea(x, 1);
                        break;
                    case "radio":
                    case "radios":         //单选框
                        _this.setRadio(x, 1);
                        break;
                    case "checkbox":
                    case "checkboxs":      //复选框
                        _this.setCheckbox(x, 1);
                        break;
                    case "autonumber":     //自动编号
                        _this.setAutonum2(x, itemcount);
                        break;
                    case "date":           //日期控件
                        _this.setDate(x, 1);
                        break;
                    case "calculation":    //计算控件
                        _this.setCalcula(x, 1);
                        break;
                    case "image":         //图片控件
                        _this.setImage(x, 1);
                        break;
                    case "upload":         //上传控件
                        _this.setUpload(x, 1);
                        break;
                    case "statistics":     //统计控件
                        _this.setStatis(x, 1);
                        break;
                    case "htmlarea":     //Html编辑器
                        _this.setHtmlarea(x, 1);
                        break;
                    case "macros":     //Html编辑器
                        _this.setMacros(x, 1);
                        break;
                }
            }
        });
    }
    , DelTableRow: function (x) {   //自增表格删除行
        var id = $(x).attr("id");
        $(".tablestyle").on("click", "tr[id^=" + id + "Sub] button[type=button].increasetabledel", function (ids, y) {
            var tr = $(this).closest("tr");
            $(tr).remove();
            FormUI.GetStatis();
            FormUI.RbgTableRow(id);
        });
    }
    , RbgTableRow: function (id) {   //更改自增行背景色
        $("[id^=" + id + "Sub]:even").css("background-color", "#F0FFFF");//奇数 
        $("[id^=" + id + "Sub]:odd").css("background-color", "");        //偶数
    }
    , PrefixInteger: function (num, length) { //产生固定位数数字，不够左边补0
        return (Array(length).join('0') + num).slice(-length);
    }
    , Createvalidation: function (data) {  //动态创建验证函数
        var _this = this;
        _this.strIncreaseValid = "";
        _this.strRealValid = "";
        _this.strSubmitValid = "";
        if (data && data.length > 0) {
            var FormByURL = FormUI.FiltData(FormUI.BaseData.initData, "FormSort", 1);
            for (var i = 0; i < data.length; i++) {

                var isvisible = data[i].IsVisible;

                if (!isvisible)  //不可见则不需要验证
                {
                    continue;
                }
                if (FormByURL && FormByURL.length > 0)   //嵌套其他模块URL，不需要进行验证
                {
                    var FormFieldIsURL = FormUI.FiltData(FormByURL, "FormVID", data[i].FormVID);
                    if (FormFieldIsURL && FormFieldIsURL.length > 0) {
                        continue;
                    }
                }

                FormUI.ControleVerifyString(data[i]);   //生成单个控件js验证语句
            }
        }
        var addIncreaseRowFunction = "function AddIncreaseRowFunction(){" + FormUI.strIncreaseValid + " return true; };";  //自增行添加时验证
        var realValidFunction = "function RealValidFunction(){" + FormUI.strRealValid + " return true; };";                //失去焦点时验证
        var submitValidFunction = "function SubmitValidFunction(){" + FormUI.strSubmitValid + " return true; };";          //提交时验证
        FormUI.LoadScriptStr(addIncreaseRowFunction + realValidFunction + submitValidFunction);                            //将js加载到页面
        RealValidFunction();
    }
    , ControleVerifyString: function (data) {
        var fieldname = data.FieldName;
        var fieldtitle = data.FieldTitle;
        var fieldtype = data.FieldType;
        var itemtype = data.ItemType;
        var fieldvalid = data.FieldValid;
        var isincrease = data.IsIncrease;
        var validStr = "", inputReg = "", inputMsg = "", beginval = "", endval = "";
        var flag = true;   //判断是否要再次添加必填语句

        if (fieldvalid == "1") {
            switch (fieldtype) {
                case "radios":
                case "checkboxs":
                    validStr = " if($('input[name=" + fieldname + "]:checked').length==0){ layer.msg('" + fieldtitle + "必须要选择项目！'); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); }  return false;}"
                    break;
                case "image":
                    validStr = "if($('#" + fieldname + "imagePath').val()=='') { layer.msg('" + fieldtitle + "需上传图片！'); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;}";
                    break;
                case "upload":
                    validStr = "if($('#" + fieldname + "filePath').val()=='') { layer.msg('" + fieldtitle + "需上传附件！'); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;}";
                    break;
                default:
                    validStr = "if($('#" + fieldname + "').val()=='') { layer.msg('" + fieldtitle + "不能为空'); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;}";
                    flag = false;
                    break;
            }
        }

        //数值控件进行数值合法性验证
        if (fieldtype == "text") {

            beginval = $("#" + fieldname).attr("orgrangebegin");
            endval = $("#" + fieldname).attr("orgrangeend");

            if (itemtype == "int") {
                inputReg = 'FormUI.regint';
                inputMsg = "数据必须为整数(范围：" + beginval + "-" + endval + ")!";
            }
            else if (itemtype == "zint") {
                inputReg = 'FormUI.regzint';
                inputMsg = "数据必须为正整数(范围：" + beginval + "-" + endval + ")!";
            }
            else if (itemtype == "fint") {
                inputReg = 'FormUI.regfint';
                inputMsg = "数据必须为负整数(范围：" + beginval + "-" + endval + ")!";
            }
            else if (itemtype == "float") {
                inputReg = 'FormUI.regfloat';
                inputMsg = FormUI.msgfloat;
            }
            else if (itemtype == "percent") {
                inputReg = 'FormUI.regpercent';
                inputMsg = FormUI.msgpercent;
            }

            if (inputReg) {
                if (fieldvalid == "1") {    //必填
                    if (flag) {
                        validStr = validStr + " if ($('#" + fieldname + "').val() == '') { layer.msg('" + fieldtitle + "不能为空！'); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false; } ";
                    }
                    validStr = validStr + "  if(!" + inputReg + ".test($('#" + fieldname + "').val())) { layer.msg('[" + fieldtitle + "] " + inputMsg + "'); $('#" + fieldname + "').val(''); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;} ";
                    if (itemtype == "int" || itemtype == "zint" || itemtype == "fint") {
                        validStr = validStr + "if($('#" + fieldname + "').val() == ''||(parseFloat($('#" + fieldname + "').val())<" + parseFloat(beginval) + "||parseFloat($('#" + fieldname + "').val())>" + parseFloat(endval) + ")){layer.msg('[" + fieldtitle + "] " + inputMsg + "'); $('#" + fieldname + "').val(''); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;} ";
                    }

                }
                else if (fieldvalid == "0") {   //可以为空
                    validStr = validStr + " if($('#" + fieldname + "').val()!=''&&!" + inputReg + ".test($('#" + fieldname + "').val())) { layer.msg('[" + fieldtitle + "] " + inputMsg + "'); $('#" + fieldname + "').val(''); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;} ";
                    if (itemtype == "int" || itemtype == "zint" || itemtype == "fint") {
                        validStr = validStr + "if($('#" + fieldname + "').val() != ''&&(parseFloat($('#" + fieldname + "').val())<" + parseFloat(beginval) + "||parseFloat($('#" + fieldname + "').val())>" + parseFloat(endval) + ")){layer.msg('[" + fieldtitle + "] " + inputMsg + "'); $('#" + fieldname + "').val(''); if(FormUI.isSubmit){ $('#" + fieldname + "').focus(); } return false;} ";
                    }
                }
            }
        }
        if ((fieldtype == "text" && itemtype == "text") || fieldtype == "textarea") {  //长度限制
            var limit = $("#" + fieldname).attr("orgtextlimit");
            if (limit && limit > 0) {
                var val = $("#" + fieldname).val();
                validStr = validStr + "if(($('#" + fieldname + "').val()).length>" + limit + "){layer.msg('[" + fieldtitle + "]内容超过规定的 " + limit + " 个字符长度限制.'); return false; }";
            }
        }

        if (isincrease == "1") {//自增列表中的控件
            FormUI.strIncreaseValid = FormUI.strIncreaseValid + validStr;    //自增行时验证
            if (fieldtype != "date" && fieldtype != "image") {
                FormUI.strRealValid = FormUI.strRealValid + "$('table.tablestyle').on('blur', '#" + fieldname + "', function () {" + validStr + " });"; //实时验证
            }

        }
        else {                  //非自增列表中的控件
            if (fieldtype != "date" && fieldtype != "image") {
                FormUI.strRealValid = FormUI.strRealValid + " $('#" + fieldname + "').on('blur',function(){" + validStr + " });";   //实时验证


            }
            FormUI.strSubmitValid = FormUI.strSubmitValid + validStr;       //提交表单时验证                                            
        }

    }
    , ScriptRequired: function (data) {   //必填js验证

    }
    , LoadScriptStr: function (code) {
        var myScript = document.createElement("script");
        myScript.type = "text/javascript";
        try {
            myScript.appendChild(document.createTextNode(code));
        }
        catch (ex) {
            myScript.text = code;
        }
        document.body.appendChild(myScript);
    }
    , GetStatis: function () {  //计算统计控件的值
        $("input[sfplugins='statistics']").each(function (i, x) {
            var id = $(x).attr("id");
            $(x).val("");
            $("#" + id + "statistics").text("");
            var orgmethod = $(x).attr("orgmethod");
            var orgdotnumber = $(x).attr("orgdotnumber"); //保留位数
            var itemid = $(x).attr("orgstatisticsitem");
            var reg = new RegExp("^-?(0|\d+)(\.\d+)?$"); //匹配数值
            var sfplugins = $("#" + itemid).attr("sfplugins");
            var trid = $("#" + itemid).closest("tr[sfplugins=increasetable]").attr("id");
            var result = 0;
            var count = 0;
            $("[id^=" + itemid + "Item][sfplugins-increasetable^=" + trid + "Sub][sfplugins-item=" + sfplugins + "]").each(function (j, y) {  //所有统计字段
                var val = $(y).val().replace(/\,/g, '');
                if (!reg.test(val)) {
                    if (val == "" || val == undefined || val == null) {
                        val = 0;
                    }
                    result = parseFloat(result) + parseFloat(val);
                    count = count + 1;
                }
                else {
                    return;
                }
            });
            if (orgmethod == "AVG" && count > 0) {
                result = result / count;
            }
            result = Number(result).toFixed(Number(orgdotnumber));  //保留小数位数

            var num1 = "", num2 = "", reg = /(-?\d+)(\d{3})/;
            if (result.indexOf('.') >= 0) {  //有小数点
                var num = result.split('.');
                if (num.length > 0) {
                    num1 = num[0].replace(reg, '$1,$2');
                    num2 = num[1];
                    result = num1 + '.' + num2;
                }
            }
            else {
                result = result.replace(reg, '$1,$2');
            }

            //result = FormUI.Comdify(result);   //增加千分号
            $(x).val(result);
            $("#" + id + "statistics").text(result);
        });
    }
    //, Comdify(n) {  //整数部分增加千分号
    //    var re = /\d{1,3}(?=(\d{3})+$)/g;
    //    var n1 = n.replace(/^(\d+)((\.\d+)?)$/, function (s, s1, s2) {
    //        return s1.replace(re, "$&,") + s2;
    //    });
    //    return n1;
    //}   
    , GetIncreaseIDList: function () {
        $("table tr[sfplugins='increasetable']").each(function (i, x) {
            strIncreaseItemIDs = ",";
            $(x).find("[sfplugins]").each(function (j, y) {
                strIncreaseItemIDs = strIncreaseItemIDs + $(y).attr("id") + ",";
            });
        });
    }
    , IsIEBrower: function () {
        var result = false;
        var version = (navigator.userAgent.match(/.+(?:rv|it|ra|ie)[V:]([\d.]+)/) || [])[1];
        var msie = /msie/.test(navigator.userAgent.toLowerCase());
        if (msie) result = true;
        if (version == '11.0') result = true;
        return result;
    }
    , LoadFlowInfo: function (tablen) {  //加载流程
        var _this = this;
        if (_this.isHistory == "1") {
            return;
        }
        $.ajax({
            type: "Post",
            dataType: "text",
            //url: "WorkflowRoute.aspx?a=" + (new Date()).getTime(),
            url: FormUI.curRootPath + "/WfFormDetails/Process",
            data: {
                FlowApplyID: _this.FlowApplyID
            },
            success: function (data) {
                $('.nav.nav-tabs').append('<li><a data-toggle="tab" alt="#tab-content' + tablen + '" href="#" title="流程进度">流程进度</a>');
                $('.tab-content').append('<div id="tab-content' + tablen + '" pagetype="flowinfo" class="tab-pane">' + data + '</div>');
                if (_this.FlowStatus == 0 || _this.FlowStatus == 2) {
                    tablen = tablen + 1;
                    FormUI.LoadFlowList(tablen);    //流程列表
                }
                else {
                    if (_this.FlowModel == 1) {    //固定流程
                        if (!navigator.userAgent.match(/mobile/i)) {
                            FormUI.LoadFlowTrack(tablen + 1);   //加载流程轨迹
                        }
                    }
                }
            }
        });
    }
    , LoadFlowList: function (tablen) {
        var _this = this;
        if (_this.isHistory == "1") {
            return;
        }
        $.ajax({
            type: "Post",
            dataType: "text",
            //url: "dialog/FlowList.aspx?a=" + (new Date()).getTime(),
            url: FormUI.curRootPath + "/WfFormDetails/List",
            data: {
                FlowApplyID: _this.FlowApplyID,
                FlowVId: _this.FlowVID
            },
            success: function (data) {
                $('.nav.nav-tabs').append('<li><a data-toggle="tab" alt="#tab-content' + tablen + '" href="#" title="流程列表">流程列表</a>');
                $('.tab-content').append('<div id="tab-content' + tablen + '" pagetype="flowlist" class="tab-pane">' + data + '</div>');
                if (_this.FlowModel == 1) {    //固定流程
                    if (!navigator.userAgent.match(/mobile/i)) {
                        FormUI.LoadFlowTrack(tablen + 1);   //加载流程轨迹
                    }
                }
            }
        });
    }
    , LoadFlowTrack: function (tablen) {
        var _this = this;
        if (_this.isHistory == "1") {
            return;
        }
        //var url = "WorkflowEditorView.aspx?FlowVID=" + flowVId + "&FlowApplyID=" + flowUseId;
        var url = FormUI.curRootPath + "/WfDefinition/EditorView?FlowVID=" + _this.FlowVID + "&FlowApplyID=" + _this.FlowApplyID;
        var iframe = $('<iframe id="iframeflowtrack" style="width:100%;height:1200px;"  frameborder="0" scrolling="no" > </iframe>');

        $('.nav.nav-tabs').append('<li><a data-toggle="tab" alt="#tab-content' + tablen + '" href="#" title="流程轨迹" id="flowgj" >流程轨迹</a>');
        var tabCon = $('<div id="tab-content' + tablen + '" pagetype="flowtrack" class="tab-pane"> </div>');
        tabCon.append(iframe);
        $('.tab-content').append(tabCon);

        var IsLoad = false;
        if (FormUI.IsIEBrower()) {
            $("#flowgj").click(function () {
                if (IsLoad) return;
                iframe.attr("src", url);
                IsLoad = true;
            });
        } else {
            iframe.attr("src", url);
        }
        //$('#iframeflowtrack').load(function () {
        //    alert('good');
        //});

    }
    , LoadFileList: function (data) {  //加载附件
        var id = data.Key;
        if (data.Value != '' && data.Value != null) {   //自增行
            if (data.Value.indexOf('|') > 0) {
                var uArray = data.Value.split('|');
                if (uArray && uArray.length > 0) {
                    var filenames = uArray[0].replace(/,$/, '').split(',');
                    var fileidids = uArray[1].replace(/,$/, '').split(',');
                    if (filenames && filenames.length > 0) {
                        for (var i = 0; i < filenames.length; i++) {
                            if (filenames[i].indexOf('.') >= 0) {
                                FormUI.AddFileListHtml(id, filenames[i]);
                            }
                            else {
                                FormUI.FileList(id, fileidids[i], filenames[i]);
                            }
                        }
                    }
                }
            }
            else if (data.Value.indexOf('.') > 0) {    //旧的数据
                if (data.Value.indexOf(",") > 0) {
                    var fileList = data.Value.split(",");
                    for (var i = 0; i < fileList.length - 1; i++) {
                        FormUI.AddFileListHtml(id, fileList[i]);   //加载附件列表
                    }
                    $("#" + id + "filePath").val(data.Value);
                }
            }
            else {                                   //新的数据
                $.ajax({
                    type: 'Post',
                    url: FormUI.curRootPath + '/WfFormDefine/GetFileIsExists',
                    data: { id: data.Value },
                    success: function (result) {
                        if (result.flag == 1) {
                            if (result.data && result.data.length > 0) {
                                for (var i = 0; i < result.data.length; i++) {
                                    var file = result.data[i];
                                    if (file) {
                                        var fileName = file.OriginalName;
                                        var fileId = file.FileSystemId;
                                        FormUI.FileList(id, fileName, fileId);
                                    }
                                }
                            }
                        }
                    }
                });
            }
            $("#" + id + "filePath").val(data.Value);
        }
    }
    , DelUploadFile: function (e) {            //删除附件
        var filePath = $(e).attr("alt");       //被删除的文件路径
        var id = $(e).attr("fileid");          //上传控件的ID
        $(e).parent().parent().remove();       //移除被删除的文件

        var filePathList = "";
        $("#" + id).parent().find(".uploadFileName").each(function (i, x) {
            filePathList += $(x).attr("alt") + ";";
        });
        $("#" + id + "filePath").val(filePathList);

        $("#" + id + "fileName").val("");
        $("#" + id).val("");

        //删除文件夹中的文件--edit
        $.ajax({
            type: 'Post',
            data: { filePath: filePath },
            url: FormUI.curRootPath + "/WfFormDefine/DeleteUpload",
            success: function (result) {
                if (result.flag = 1) {
                    layer.msg(result.msg);
                    //alert(result.msg);
                }
                else {
                    layer.msg(result.msg);
                    //alert(result.msg);
                    return;
                }
            }
        });
    }
    , SetFieldIsShow: function (data) {
        if (data || data.length == 0) {
            return;
        }
        for (var i = 0; i < data.length; i++) {
            if ($("#" + data[i].FieldName)) {         //控件存在
                var IsVisible = data[i].IsVisible;    //可见
                var IsReadOnly = data[i].IsReadOnly;  //是否只读
                var IsIncrease = data[i].IsIncrease;  //是否为自增列表控件
                var FieldType = data[i].FieldType;    //控件类型 
                var FieldName = data[i].FieldName;    //控件编号
                //if (FieldType == "increasetable")     //自增列不隐藏
                //{
                //    continue;
                //}
                if (!IsVisible) {    //不可见
                    if (FieldType == "image") {           //图片控件
                        $("#" + FieldName + "preview").remove();
                        $("#" + FieldName + "imagePath").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "upload") {   //上传控件
                        $("#" + FieldName).parent(".fileBox").remove();
                    }
                    else if (FieldType == "autonumber")  //自动编号框
                    {
                        $("#" + FieldName + "number").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "calculation")  //计算框
                    {
                        $("#" + FieldName + "calcula").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "htmlarea")  //html文本框
                    {
                        $("#" + FieldName + "editor").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "userselect")  //人员选择框
                    {
                        $("#" + FieldName + "macros").remove();
                        $("#" + FieldName + "code").remove();
                        $("#" + FieldName + "selectbutton").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "macros") {
                        $("#" + FieldName + "macros").remove();
                        $("#" + FieldName + "code").remove();
                        $("#" + FieldName + "selectbutton").remove();
                        $("lable[name=" + FieldName + "]").remove();
                        $("#" + FieldName).remove();
                    }
                    else if (FieldType == "office") {
                        $("#" + FieldName + "div").remove();
                        $("a[forDiv=" + FieldName + "div]").remove();

                    }
                    else if (FieldType == "increasetable") {
                        $(".tablestyle").find("tr[id=" + FieldName + "]").remove();
                        $(".tablestyle").find("tr[id^=" + FieldName + "Sub]").remove();;
                    }
                    else if (FieldName == "WorkFlowNum") {
                        $("#WorkFlowNum").attr("readonly", "readonly");
                    }
                    else {
                        $("#" + FieldName).remove();      //移除控件
                    }

                    if (IsIncrease == 1) {            //自增列要移除明细项
                        $("[id^=" + FieldName + "Item]").remove();
                    }
                }
                else {    //可见后，才能判断是否只读
                    if (IsReadOnly) {
                        if (FieldType == "radios") {
                            $("#" + FieldName + " input[type=radio][name=" + FieldName + "]").attr("disabled", true);
                        }
                        else if (FieldType == "checkboxs") {
                            $("#" + FieldName + " input[type=checkbox][name=" + FieldName + "]").attr("disabled", true);
                        }
                        else if (FieldType == "date" || FieldType == "select" || FieldType == "upload") {
                            $("#" + FieldName).attr("disabled", true);    //时间空间设置为readonly无效，只能设置为disabled
                        }
                        else if (FieldType == "userinfo")   //用户信息控件
                        {
                            $("#" + FieldName).attr("readonly", "readonly");        //设为只读
                            $("#" + FieldName + "search").attr("disabled", true);
                        }
                        else if (FieldType == "macros" || $("#" + FieldName).attr("sfplugins") == "userselect")   //宏控件人员选择框
                        {
                            $("#" + FieldName + "selectbutton").attr("disabled", true);
                        }
                        else if (FieldType == "image") {
                            $("#" + FieldName).attr("disabled", true);
                        }
                        else if (FieldType == "increasetable") {
                            $(".tablestyle").off("dblclick", "tr[id^=" + FieldName + "]");

                            $(".tablestyle").off("click", "tr[id=" + FieldName + "] button[type=button].increasetableadd");
                            $(".tablestyle").off("click", "tr[id^=" + FieldName + "Sub] button[type=button].increasetabledel");
                            $('button[type=button].increasetableadd').remove();
                            $('button[type=button].increasetabledel').remove();
                        }
                        else {
                            $("#" + FieldName).attr("readonly", "readonly");    //设为只读
                        }
                    }
                    else {
                        $("#" + FieldName).removeAttr("readonly");          //移除只读
                    }
                }

            }
        }
    }
    , SetControlIsRead: function (id) {
        FormUI.BindControlEvent(id);   //绑定控件的事件
        if (!FormUI.BaseData || !FormUI.BaseData.FieldPower || FormUI.BaseData.FieldPower.length == 0) {
            return;
        }
        var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id);  //获取控件的权限
        FormUI.SetControlIsReadElement(data);
    }
    , SetIncreaseIsRead: function (ids) {    //设置自增行内的空间只读
        var id = ids.replace(/;$/g, "").split(";");
        var count = id.length;
        if (count > 0) {
            for (var i = 0; i < count; i++) {
                var data = FormUI.FiltData(FormUI.BaseData.FieldPower, "FieldName", id[i]);  //获取控件的权限
                FormUI.SetControlIsReadElement(data);
            }
        }

    }
    , SetControlIsReadElement: function (data) {
        var _this = this;
        if (data && data.length > 0) {
            var IsVisible = data[0].IsVisible;    //可见
            var IsReadOnly = data[0].IsReadOnly;  //是否只读
            var IsIncrease = data[0].IsIncrease;  //是否为自增列表控件
            var FieldType = data[0].FieldType;    //控件类型 
            var FieldName = data[0].FieldName;    //控件编号
            if (_this.FlowPage == 0) {   //查看
                IsReadOnly = 1;
            }
            if (!IsVisible) {    //不可见
                if (FieldType == "image") {           //图片控件
                    $("#" + FieldName + "preview").remove();
                    $("#" + FieldName + "imagePath").remove();
                    $("#" + FieldName).remove();
                }
                else if (FieldType == "upload") {   //上传控件
                    $("#div" + FieldName).remove();
                    $("#" + FieldName + "fileList").remove();
                    //$("#" + FieldName).remove();
                    //$("#" + FieldName + "filePath").remove();
                    //$("#" + FieldName + "fileName").remove();
                    //$("#btnSelect" + FieldName).remove();
                    $("#" + FieldName + "fileList").remove();
                    if (IsIncrease == 1) {
                        //$("[id^='" + FieldName + "Item']").remove();
                        //$("[id^='btnSelect" + FieldName + "Item']").remove();
                        $("[id^='div" + FieldName + "Item'").remove();
                        $("[id^='" + FieldName + "fileListItem").remove();
                    }

                }
                else if (FieldType == "autonumber")  //自动编号框
                {
                    $("#" + FieldName + "number").remove();
                    $("#" + FieldName).remove();
                }
                else if (FieldType == "calculation")  //计算框
                {
                    $("#" + FieldName + "calcula").remove();
                    $("#" + FieldName).remove();
                }
                else if (FieldType == "htmlarea")  //html文本框
                {
                    $("#" + FieldName + "editor").remove();
                    $("#" + FieldName).remove();
                }
                else if (FieldType == "macros") {
                    var type = $("#" + FieldName).attr("orgtype");
                    if (type == 2) {   //人员选择框
                        $("#" + FieldName + "UserCode").remove();
                        $("#" + FieldName + "UserName").remove();
                        $("#" + FieldName + "selectButton").remove();
                        $("#" + FieldName).remove();
                    }
                    else {
                        $("lable[name=" + FieldName + "]").remove();
                        $("#" + FieldName).remove();
                    }
                }
                else if (FieldType == "office") {
                    $("#" + FieldName + "div").remove();
                    $("a[forDiv=" + FieldName + "div]").remove();

                }
                else if (FieldType == "increasetable") {
                    $("tr[id=" + FieldName + "]").remove();
                    $("tr[id^=" + FieldName + "Sub]").remove();;
                }
                else if (FieldName == "WorkFlowNum") {
                    $("#WorkFlowNum").attr("readonly", "readonly");
                }
                else {
                    $("#" + FieldName).remove();      //移除控件
                }
                $("span[for='" + FieldName + "']:contains('*')").remove();

                if (IsIncrease == 1) {            //自增列要移除明细项
                    $("[id^=" + FieldName + "Item]").remove();
                }
            }
            else {    //可见后，才能判断是否只读
                if (IsReadOnly || FormUI.FlowStatus == 0) {
                    if (FieldType == "radios") {
                        $("#" + FieldName + " input[type=radio][name=" + FieldName + "]").attr("disabled", true);
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item'] input[type=radio][name^='" + FieldName + "Item']").attr("disabled", true);
                        }
                    }
                    else if (FieldType == "checkboxs") {
                        $("#" + FieldName + " input[type=checkbox][name=" + FieldName + "]").attr("disabled", true);
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item'] input[type=checkbox][name^='" + FieldName + "Item']").attr("disabled", true);
                        }
                    }
                    else if (FieldType == "date" || FieldType == "select" || FieldType == "upload") {
                        $("#" + FieldName).attr("disabled", true);    //时间空间设置为readonly无效，只能设置为disabled
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item']").attr("disabled", true);
                        }
                    }
                    else if (FieldType == "userinfo")   //用户信息控件
                    {
                        $("#" + FieldName).attr("readonly", "readonly");        //设为只读
                        $("#" + FieldName + "search").attr("disabled", true);
                    }
                    else if (FieldType == "macros")   //宏控件人员选择框
                    {
                        $("#" + FieldName + "selectButton").attr("disabled", true);
                        $("#" + FieldName).attr("readonly", "readonly");
                        $("#" + FieldName).attr("disabled", true);
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item'][id$=selectButton]").attr("disabled", true);
                            $("[id^='" + FieldName + "Item']").attr("disabled", true);
                        }
                    }
                    else if (FieldType == "image") {
                        //$("#" + FieldName).remove();
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item']").attr("disabled", true);
                        }
                    }
                    else if (FieldType == "increasetable") {
                        $(".tablestyle").off("click", "tr[id=" + FieldName + "] button[type=button].increasetableadd");
                        $(".tablestyle").off("click", "tr[id^=" + FieldName + "Sub] button[type=button].increasetabledel");
                        $('button[type=button].increasetableadd').remove();
                        $('button[type=button].increasetabledel').remove();
                    }
                    else {
                        $("#" + FieldName).attr("readonly", "readonly");    //设为只读
                        if (IsIncrease == 1) {
                            $("[id^='" + FieldName + "Item']").attr("readonly", "readonly");
                        }
                    }
                }
                else {
                    $("#" + FieldName).removeAttr("readonly");          //移除只读
                    if (IsIncrease == 1) {
                        $("[id^='" + FieldName + "Item']").removeAttr("readonly");
                    }
                }
            }
        }
    }
    , ToggleOffice: function (e) {  //点击展开或折叠Office
        var obj = $(e).attr("forDiv");
        if ($("#" + obj)) {
            $("#" + obj).toggle();
        }
    }
    , ShowUserList: function (userinput) {  //人员选择框（宏控件）
        userinputid = userinput;
        var path = "/";
        if (typeof window.top.rootPath != "undefined") {
            path = window.top.rootPath;
        }
        else {
            try {
                var myOpener = window.opener;
                if (typeof myOpener != "undefined") {
                    if (typeof myOpener.top.rootPath != "undefined") {
                        path = myOpener.top.rootPath;
                    }
                }
            } catch (e) {
            }
        }
        var strURL = path + "users/SelectUsers.aspx?single=false&showAll=true&txtCode=" + userinput + "code&txtName=" + userinput + "macros&callback=FormUI.SetVal";
        openNewDiv(strURL, 500, 475);//兼容浏览器的弹出选择人员框
        $("#tmpdiv").css("z-index", "99999999");
    }
    , UserDialog: function (id) {
        var users = $("#" + id + "UserCode").val();
        var callback = function (data) {
            var userids = "", usernames = "", uservalues = "";
            if (data != null && data.length > 0) {
                for (var i = 0; i < data.length; i++) {
                    userids = userids + data[i].UserSystemId + ',';
                    usernames = usernames + data[i].UserName + ',';
                }
                userids = userids.TrimEnd(',');
                usernames = usernames.TrimEnd(',');
            }
            if (userids != '' && usernames != '') {
                uservalues = userids + '_' + usernames;
            }
            $("#" + id).val(uservalues);
            $("#" + id + "UserCode").val(userids);
            $("#" + id + "UserName").val(usernames);
        };
        SFselectUser.open(users, "code", callback);
    }
    , SetVal: function () {  //用户处理存取人员信息的格式
        var userCode = $("#" + userinputid + 'code');
        var userName = $("#" + userinputid + 'macros');
        userName.attr("useridlist", userCode.val());
        var strvalue = userCode.val() + "_" + userName.val();
        $("#" + userinputid).val(strvalue);
    }
    , BindControlEvent: function (id) {  //绑定控件的事件
        var orgeventbind = $("#" + id).attr("orgeventbind");
        if (orgeventbind) {
            var bind = orgeventbind.split(',');
            if (bind && bind.length > 0) {
                $.each(bind, function (i, x) {
                    var event = x.split('|');
                    if (event && event.length > 0) {
                        FormUI.BindEvent(id, event[0], event[1]);  //绑定事件
                    }
                });
            }
        }
    }
    , BindEvent: function (id, eventname, functionname) {  //绑定js事件
        $("#" + id).on(eventname, function () {
            try {
                eval(functionname);
            } catch (e) { };
        });
    }
    , GetDataByPort: function (url, type, data) {
        $.ajax({
            url: url,
            type: type,
            data: data,
            dataType: 'json',
            success: function (result) {
                return result;
            },
            error: function (e) {
                layer.msg("通过接口" + url + "获取数据失败");
                //alert("通过接口" + url + "获取数据失败");
                return;
            }
        });

    }
    , BindData: function (id, sfplugins) {    //绑定数据        
        var x = $("#" + id);
        var param = null;
        var increasecontrol = x.attr("increasecontrol");
        var orgbindtype = x.attr("orgbindtype");//Fix 固定源 source查询sql语句  api 接口
        if (orgbindtype == "fix" || orgbindtype == undefined || orgbindtype == "" || orgbindtype == null) {
            if (sfplugins != "select") {
                FormUI.AgainStyle(x);            //重新加载样式
            }
            if (increasecontrol != 1) {
                FormUI.fillData(id, sfplugins);     //填充数据
            }
            FormUI.SetControlIsRead(id);        //设置控件的权限
        }
        else {
            if (orgbindtype == "source") {
                var orgsourcetsql = x.attr("orgsourcetsql");
                var orgsourcetable = x.attr("orgsourcetable");
                var orgsourcetext = x.attr("orgsourcetext");
                var orgsourcevalue = x.attr("orgsourcevalue");
                param = { type: 'Post', url: "FormAction.ashx?rad=" + Math.random(), data: { 'action': 'GetDataBySource', 'orgstable': orgsourcetable, 'orgsvalue': orgsourcevalue, 'orgstext': orgsourcetext } };

            }
            else if (orgbindtype == "tsql") {
                var orgsourcetsql = x.attr("orgsourcetsql");
                param = { type: "Post", url: "FormAction.ashx?rad=" + Math.random(), data: { 'action': 'GetDataBySql', 'strSql': encodeURI(orgsourcetsql) } };
            }
            else if (orgbindtype == "api") {
                var orgapiname = x.attr("orgapiname");
                param = { type: "Post", url: orgapiname };
            }
            if (param != null) {
                param.callback = function (db) {
                    if (db.IsSuccess > 0) {
                        var fieldvalue = db.FieldValue;
                        var fieldtext = db.FieldText;
                        if (sfplugins == "select") {
                            FormUI.DropBind(id, db.result, "请选择", fieldtext, fieldvalue);     //绑定下拉框
                        }
                        else {
                            FormUI.SelectBind(id, db.result, fieldtext, fieldvalue, sfplugins.replace(/s$/gi, ''));  //绑定单选复选
                            FormUI.AgainStyle(x);  //重新加载样式
                        }
                        if (increasecontrol != 1) {
                            FormUI.fillData(id, sfplugins); //填充数据
                        }
                    }
                }
                FormUI.GetAjaxData(param);   //获取下拉框数据源
                FormUI.SetControlIsRead(id); //设置控件的权限
            }
            else {
                FormUI.SetControlIsRead(id); //设置控件的权限
                FormUI.AgainStyle(x);  //重新加载样式
                if (increasecontrol != 1) {
                    FormUI.fillData(id, sfplugins); //填充数据
                }
            }
        }
    }
    , VerifyLength: function (id, sfplugins) {   //验证文本框超过长度限制
        $("#" + id).on("blur", function () {
            var limit = $("#" + id).attr("orgtextlimit");
            var val = null;
            switch (sfplugins) {
                case "text":
                case "textarea":
                    val = $("#" + id).val();
                    break;
                default:
                    val = $("#" + id).val();
                    break;
            }
            var len = val.length;
            if (len > limit) {
                var title = $("#" + id).attr("title");
                layer.msg(title + " 内容超过规定的 " + limit + " 字符长度限制.");
                //alert(title + " 内容超过规定的 " + limit + " 字符长度限制.");
                return;
            }
        });
    }
    , SetIframeFieldPower: function (data, _$) {
        var _this = this;
        var IsVisible = data.IsVisible;    //可见
        var IsReadOnly = data.IsReadOnly;  //是否只读
        var IsIncrease = data.IsIncrease;  //是否为自增列表控件
        var FieldType = data.FieldType;    //控件类型 
        var FieldName = data.FieldName;    //控件编号

        _$.find("[id='" + FieldName + "']").attr("sfplugin", FieldType);
        if (!IsVisible) {    //不可见
            if (FieldType == "radios") {
                _$.find("input[name='" + FieldName + "'][type='radio']").remove();
            }
            else if (FieldType == "checkboxs") {
                _$.find("input[name='" + FieldName + "'][type='checkbox']").remove();
            }
            else {
                _$.find("[id='" + FieldName + "']").remove();
            }
        }
        else {    //可见后，才能判断是否只读
            if (IsReadOnly || _this.FlowStatus == 0) {
                if (FieldType == "radios") {
                    _$.find("input[type='radio'][name='" + FieldName + "']").attr("disabled", true);
                }
                else if (FieldType == "checkboxs") {
                    _$.find("input[type='checkbox'][name='" + FieldName + "']").attr("disabled", true);
                }
                else if (FieldType == "date" || FieldType == "select" || FieldType == "upload") {
                    _$.find("[id='" + FieldName + "']").attr("disabled", true);       //时间空间设置为readonly无效，只能设置为disabled
                }
                else {
                    _$.find("[id='" + FieldName + "']").attr("readonly", "readonly"); //设为只读
                }
            }
        }

    }
    , IframeLoadAfter: function (FormVId) {    //设置iframe中的字段属性
        var _this = this;
        var iframeField = FormUI.FiltData(FormUI.BaseData.FieldPower, "FormVId", FormVId); //iframe中的表单字段
        var iframeid = "Sfpluginiframe" + FormVId;
        $("#" + iframeid).load(function () {   //iframe加载完成后处理
            var iframeBody = $(this).contents().find("body");
            var h = iframeBody[0].scrollHeight + 20;
            iframeBody.css("overflow-x", "hidden");
            $(this).css({ "height": h + "px" });

            $("#btnSave").attr("disabled", false);
            var _$ = $("#" + iframeid).contents();
            for (var i = 0; i < iframeField.length; i++) {
                FormUI.SetIframeFieldPower(iframeField[i], _$);  //设置iframe中字段是否可见和只读
            }

            if (_this.FlowPage != 0) {
                $("#formFooter").show();
            }
        });
    }
    , RoleList: function (data) {
        var options = '';
        if (data != null && data.length > 0) {
            for (var i = 0; i < data.length; i++) {
                options += '<option value="' + data[i].Names + '">' + data[i].Names + '</option>';
            }
        }
        return options;
    }
    , DeptList: function (data) {
        var options = '';
        var parent = FormUI.FiltData(data, 'ParentId', -1);
        if (parent != null && parent.length > 0) {
            for (var i = 0; i < parent.length; i++) {
                options = options + '<option value="' + parent[i].ChineseName + '">' + parent[i].ChineseName + '</option>';
                var rank = 0;
                options += FormUI.GetChild(data, parent[i].Id, rank, options);
            }
        }
        return options;
    }
    , GetChild: function (data, upid, rank) {
        var options = "";
        var child = FormUI.FiltData(data, 'ParentId', upid);
        if (child != null && child.length > 0) {
            for (var i = 0; i < child.length; i++) {
                options = options + '<option value="' + child[i].ChineseName + '">' + FormUI.GetRank(rank) + child[i].ChineseName + '</option>';
                options += FormUI.GetChild(data, child[i].Id, rank + 1);
            }
        }
        return options;
    }
    , GetRank: function (rank) {
        var strLevels = "";
        if (rank > 0) {
            for (var i = 0; i < rank; i++) {
                strLevels = strLevels + "│";
            }
        }
        return strLevels + "├";
    }
    , ChangeFlowStep: function () {
        var _this = this;
        if (_this.FlowModel == 2) {   //自由流程
            $("#FlowStep").change(function () {
                var flowstep = $("#FlowStep").val();
                if (flowstep == "End")            //结束
                {
                    _this.showArray[4] = 0;
                    _this.showArray[5] = 0;
                    _this.showArray[6] = 0;
                    _this.showArray[7] = 0;
                }
                else if (flowstep == "Notice") {  //通知并结束
                    _this.showArray[4] = 1;
                    _this.showArray[5] = 1;
                    _this.showArray[6] = 0;
                    _this.showArray[7] = 0;
                }
                else {                            //审批
                    _this.showArray[4] = 1;
                    _this.showArray[5] = 1;
                    _this.showArray[6] = 1;
                    _this.showArray[7] = 1;
                }
                _this.SetShow();
            });
        }
    }
    , HandleShow: function () {
        var _this = this;
        _this.showArray = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
        if (navigator.userAgent.match(/mobile/i)) {   //手机端隐藏打印
            $("#lbPrintJd").hide();
            $("#lbPrintLb").hide();
            $("#btnReview").hide();
        }
        if (_this.FlowModel == 1)  //固定流程
        {
            if (_this.FlowStatus == 1) {   //发送
                _this.showArray = [0, 0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 0, 0, 0, 0];
                if (_this.FlowIsDraft == 1) {
                    _this.showArray[11] = 1; //删除草稿
                }
            }
            else if (_this.FlowStatus == 2) {  //待办
                _this.showArray = [1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 0, 0, 0];
            }
            else {
                _this.showArray = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            }
        }
        else {  //自由流程 -1预览 0查看 1发送  2待办
            if (_this.FlowStatus == 1) {   //发送
                _this.showArray = [1, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0];
                if (_this.FlowIsDraft == 1) {
                    _this.showArray[11] = 1; //删除草稿
                }
            }
            else if (_this.FlowStatus == 2) {  //待办
                _this.showArray = [1, 1, 1, 0, 0, 0, 0, 0, 1, 1, 0, 0, 1, 0, 0];
            }
            else {
                _this.showArray = [0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0];
            }
        }

        if (_this.FlowStatus == 2) {  //待办
            if (_this.FlowModel == 1) {  //固定流程
                _this.GetFixIsClose();   //中止
            }
            _this.GetRecall();           //撤回
        }
        else {
            _this.SetShow();
        }
    }
    , SetShow: function () {
        var _this = this;

        _this.showArray[0] ? $("#flowhandle").show() : $("#flowhandle").hide();
        _this.showArray[1] ? $("#divFlowResult").show() : $("#divFlowResult").hide();   //处理结果
        _this.showArray[2] ? $("#divFlowRemark").show() : $("#divFlowRemark").hide();   //处理意见
        _this.showArray[3] ? $("#divFlowStep").show() : $("#divFlowStep").hide();       //下一步处理事件
        _this.showArray[4] ? $("#divFlowUser").show() : $("#divFlowUser").hide();       //下一步处理人员
        _this.showArray[5] ? $("#divFlowNotice").show() : $("#divFlowNotice").hide();   //通知方式
        _this.showArray[6] ? $("#divFlowRule").show() : $("#divFlowRule").hide();       //处理规则
        _this.showArray[7] ? $("#divFlowOrder").show() : $("#divFlowOrder").hide();     //处理顺序

        _this.showArray[8] ? $("#divBtn").show() : $("#divBtn").hide();
        _this.showArray[9] ? $("#btnSave").show() : $("#btnSave").hide();
        _this.showArray[10] ? $("#btnDraft").show() : $("#btnDraft").hide();
        _this.showArray[11] ? $("#btnDelete").show() : $("#btnDelete").hide();

        _this.showArray[12] ? $("#labClose").show() : $("#labClose").hide();
        _this.showArray[13] ? $("#labCancel").show() : $("#labCancel").hide();
        _this.showArray[14] ? $("#labRecall").show() : $("#labRecall").hide();
        //if ($("input[type='radio'][name='FlowResult']:checked").val() == "5") {
        //    _this.showArray[14] ? $("#labRecall").show() : $("#labRecall").hide();
        //}
        //else {
        //    $("#labRecall").hide();
        //}
        if (_this.FlowStatus == -1 || _this.FlowStatus == 1) {  // 预览和新建不能打印
            $("#divPrint").hide();
        }
        else {
            $("#divPrint").show();
        }
    }
    , ChangeResult: function () {
        var _this = this;
        if (_this.FlowModel == 2 && _this.FlowStatus == 2)   //自由流程待办
        {
            $.ajax({
                type: "Get",
                url: FormUI.curRootPath + "/WfFormDetails/GetFreeHandleShow",
                data: { FlowApplyID: _this.FlowApplyID, FlowApplyStepID: _this.FlowApplyStepID },
                success: function (data) {
                    if (data) {
                        _this.ChangeResultShow();
                    }
                }
            });
        }
        else if (_this.FlowModel == 1 && _this.FlowStatus == 2) {  //固定流程待办
            _this.ChangeResultFixRecallShow();
        }
    }
    , ChangeResultShow: function () {
        var _this = this;
        $('[type="radio"][name="FlowResult"]').on('change', function () {
            var val = $('input[type="radio"][name="FlowResult"]:checked').val();
            if (val == 1)  //通过
            {
                _this.showArray[3] = 1;     //下一步处理事件
                var flowstep = $("#FlowStep").val();
                if (flowstep != 'End') {
                    _this.showArray[4] = 1; //下一步处理人员
                    _this.showArray[5] = 1;  //通知方式
                    if (flowstep != 'Notice') {
                        _this.showArray[6] = 1;  //处理规则
                        _this.showArray[7] = 1;  //处理顺序
                    }
                }
            }
            else {        //不通过
                _this.showArray[3] = 0;
                _this.showArray[4] = 0;
                _this.showArray[5] = 0;
                _this.showArray[6] = 0;
                _this.showArray[7] = 0;
                if (val == 5) {  //撤回
                    _this.showArray[14] = 1;
                }
                else {
                    _this.showArray[14] = 0;
                }
            }
            _this.SetShow();
        });
    }
    , ChangeResultFixRecallShow: function () {
        var _this = this;
        $('[type="radio"][name="FlowResult"]').on('change', function () {
            var val = $('input[type="radio"][name="FlowResult"]:checked').val();
            if (val == 5) {  //撤回
                _this.showArray[14] = 1;
            }
            else {
                _this.showArray[14] = 0;
            }
            _this.SetShow();
        });
    }
    , GetFixIsClose: function () {  //设置固定流程中止权限
        var _this = this;
        if (_this.FlowModel == 1 && _this.FlowStatus == 2) {  //固定流程待办
            $.ajax({
                type: 'Get',
                url: FormUI.curRootPath + '/WfFlowSend/GetFixIsClose',
                data: { FlowVID: _this.FlowVID, FlowNodeID: _this.FlowNodeID },
                success: function (data) {
                    if (data == 1) {
                        _this.showArray[12] = 1;
                        _this.SetShow();
                    }
                }
            });
            //_this.showArray[12] = 1
        }
    }
    , GetRecall: function () {  //绑定撤回列表
        var _this = this;
        $.ajax({
            type: "Get",
            url: FormUI.curRootPath + "/WfFlowSend/GetRecall",
            data: { FlowApplyID: _this.FlowApplyID, FlowApplyStepID: _this.FlowApplyStepID },
            success: function (result) {
                if (result.flag == 1) {
                    var option = '';
                    var list = result.data;
                    if (list != null && list.length > 0) {
                        for (var i = 0; i < list.length; i++) {
                            option += '<option value="' + list[i].Id + '">' + list[i].StepName.toString().replace(/节点:/, "") + '</option>';
                        }
                        $("#FlowRecall").append(option).select2({ minimumResultsForSearch: -1, width: '150px' });  //绑定撤回节点列表
                        _this.showArray[13] = 1;
                    }
                }
                _this.SetShow();
            }
        });
    }
    , Recall: function (saveway) {  //撤回
        var _this = this;
        layer.confirm('确定要撤回该流程？', { icon: 3 }, function (index) {
            layer.close(index);
            $.ajax({
                type: 'Post',
                url: FormUI.curRootPath + '/WfFlowSend/Recall',
                data: {
                    FlowApplyId: _this.FlowApplyID,
                    FlowApplyStepId: _this.FlowApplyStepID,
                    RecallId: $("#FlowRecall").val(),
                    Remark: encodeURIComponent($('#FlowRemark').val())
                },
                success: function (result) {
                    if (result.flag == 1) {
                        _this.ClosePage(saveway, result.data);
                        return;
                    }
                    else {
                        layer.msg(result.msg);
                        return;
                    }
                }
            });
        });
    }
    , LoadDraft: function () {   //加载草稿信息
        var _this = this;
        if (_this.FlowStatus == 1) {
            $.ajax({
                type: 'Get',
                url: FormUI.curRootPath + '/WfFlowSend/LoadDraft',
                data: { FlowApplyID: _this.FlowApplyID },
                success: function (data) {
                    if (data) {
                        var arr = data.split('|');
                        if (arr != null && arr.length >= 0) {
                            $("#FlowStep").val(arr[0]).trigger("change");  //下一步处理事件
                            $("#FlowUserCode").val(arr[1]);                //下一步处理人员
                            $("#FlowUserName").val(arr[2]);
                            $("[type='checkbox'][name='FlowNotice']").each(function (i, x) {  //通知方式
                                var val = $(x).val();
                                if (val != "Message") {
                                    if (arr[3].indexOf(val) >= 0) {
                                        $(x).attr("checked", true);
                                    }
                                    else {
                                        $(x).attr("checked", false);
                                    }
                                }
                            });
                            $("[type='radio'][name='FlowRule'][value='" + arr[4] + "']").attr("checked", true);   //处理规则
                            $("[type='radio'][name='FlowOrder'][value='" + arr[5] + "']").attr("checked", true)   //处理顺序
                        }
                    }
                }
            });

        }
    }
    , SaveForm: function (saveway, e) {   //saveway 0 1 2 3中止
        var _this = this;
        if (_this.isSaveing) {
            return;
        }
        _this.isSaveing = true;
        $(e).attr("disabled", true);  //锁定按钮
        var val = $("input[type='radio'][name='FlowResult']:checked").val();  //审核意见

        if (saveway == 1 && _this.FlowStatus == 2) {
            saveway = 2;  //待办
        }

        if (val == 3) {   //中止
            saveway = 3;
            _this.CloseFlow(saveway);
            return;
        }
        else if (val == 5) {  //撤回
            saveway == 5;
            _this.Recall(5);
            return;
        }

        if (saveway == 1 || saveway == 2) {
            if (!_this.FlowValid(e)) { //验证
                $(e).attr("disabled", false);
                _this.isSaveing = false;
                return;
            }
        }
        else if (saveway == 0) { //草稿
            if (!_this.FlowValidDraft(e)) { //验证
                $(e).attr("disabled", false);
                _this.isSaveing = false;
                return;
            }
        }
        var Increase = JSON.stringify(_this.GetIncreaseInfo());              //获取自增行数据
        var FormFieldsPower = JSON.stringify(_this.BaseData.FieldPower);//表单字段权限
        $('#formShow').ajaxSubmit({
            type: 'Post',
            url: FormUI.curRootPath + '/WfFlowSend/SaveFlow',
            data: {
                SaveWay: saveway,
                FlowModel: _this.FlowModel,
                FlowApplyStepID: _this.FlowApplyStepID,
                FlowStatus: _this.FlowStatus,
                FlowNodeID: _this.FlowNodeID,
                FormFieldsPower: FormFieldsPower,
                Increase: Increase
            },
            async: false,
            success: function (result) {
                if (result.flag == 1) {  //提交成功
                    var data = result.data;
                    if (data.IsEnd) {
                        _this.SaveModuleData(saveway, data);  //保存模块数据(流程结束)
                    }
                    else {
                        _this.ClosePage(saveway, data.MenuID);    //关闭页面
                    }
                }
                else {
                    layer.msg(result.msg);
                    $(e).attr("disabled", false);  //解锁按钮
                    _this.isSaveing = false;
                    return;
                }

            }
        });
    }
    , SaveModuleData: function (saveway, _data) {
        $(".tab-content div[id^=tab-content].tab-pane[SfpluginFormNumber]").each(function (i, x) {
            var formvid = $(x).attr("SfpluginFormNumber");
            if (formvid) {
                var url = $(x).find("input[sfplugins=save]").attr("orgurl");
                if (!url) {
                    var iframe = $(x).find("iframe[id='Sfpluginiframe" + formvid + "']");  //获取嵌套iframe
                    if (iframe) {
                        url = $(iframe).contents().find("input[sfplugins=save]").attr("orgurl");      //获取url
                    }
                }
                if (url) {
                    FormUI.count1 = FormUI.count1 + 1;
                    $.ajax({
                        type: 'Post',
                        url: url,
                        data: {
                            'ItemID': _data.ItemID
                              , 'ItemFormData': _data.FormData
                              , 'FlowApplyID': FormUI.FlowApplyID
                              , 'ItemStatus': _data.ItemStatus
                              , 'RepealStatus': _data.RepealStatus
                              , 'RepealCode': _data.RepealCode
                        },
                        success: function (r) {
                            if (r.flag == 1) {
                                FormUI.count2 = FormUI.count2 + 1;
                            }
                            else {
                                FormUI.isSaveSuccess = false;
                                FormUI.Msg = r.msg;
                            }

                        },
                        error: function () {
                            FormUI.isSaveSuccess = false;
                            FormUI.Msg = r.msg;
                        }

                    });
                }
            }
        });
        FormUI.listenResult = setInterval("FormUI.GetSaveResult(" + saveway + "," + _data.MenuID + ")", 1000);
    }
    , GetSaveResult: function (saveway, MenuID) {
        var _this = this;
        if (FormUI.count1 == FormUI.count2) {
            clearInterval(FormUI.listenResult);
            _this.ClosePage(saveway, MenuID);
            return;
        }
        if (FormUI.isSaveSuccess == false) {
            clearInterval(FormUI.listenResult);
            if (FormUI.Msg) {
                layer.msg(FormUI.Msg);
                //alert(FormUI.Msg);
            }
            else {
                layer.msg('保存失败');
                //alert("保存失败");
            }
            return;
        }
    }
    , CloseCurrenPage: function () {
        if (navigator.userAgent.indexOf("isoffice") > -1) {   //判断是否是在手机端             
            if (navigator.userAgent.indexOf("iosVersion") > -1) {
                window.location.href = "iosCommit";  //苹果手机端添加捕获事件
            }
            window.nativeApi.closeWindow();      //Android端关闭页面
            return false;
        }
        try {
            parent.homeCommon.TaskModal.hide();/*关闭工作台待办弹出框*/
            window.parent.SF.tab.closeCurrent();  //关闭当前页面
        }
        catch (ex) {

        }
    }
    , ClosePage: function (saveway, MenuID, msg) {
        if (navigator.userAgent.indexOf("isoffice") > -1) {   //判断是否是在手机端     
            //layer.msg('提交成功');  //手机端不支持
            alert("提交成功");
            if (navigator.userAgent.indexOf("iosVersion") > -1) {
                window.location.href = "iosCommit";  //苹果手机端添加捕获事件
            }
            window.nativeApi.closeWindow();      //Android端关闭页面
            return false;
        }
        var MenuName = "", MenuURL = "", Msg = "";
        if (saveway == 0) {
            MenuName = "待办"
            MenuURL = "WfFlowTodo/Index";
            Msg = "成功保存为草稿！";
        }
        else if (saveway == 1) {
            MenuName = "已发";
            MenuURL = "WfFlowSend/Send";
            Msg = "提交成功！";
        }
        else if (saveway == 2) {
            MenuName = "已办";
            MenuURL = "WfFlowTodo/Todo";
            Msg = "提交成功！";
        }
        else if (saveway == 3) {
            MenuName = "已办";
            MenuURL = "WfFlowTodo/Todo";
            Msg = "中止成功！";
        }
        else if (saveway == 4) {
            MenuName = "待办"
            MenuURL = "WfFlowTodo/Index";
            Msg = "删除草稿成功！";
        }
        else if (saveway == 5) {
            MenuName = "已办";
            MenuURL = "WfFlowTodo/Todo";
            Msg = "撤回成功！";
        }
        else {
            MenuName = "已办";
            MenuURL = "WfFlowTodo/Todo";
            Msg = "成功保存为草稿！";
        }
        if (msg) {
            Msg = msg;
        }
        layer.confirm(Msg, { btn: ['确定'], icon: 1 }, function (index) {
            layer.close(index);
            var p = $(window.parent.document).find("#tab-box-list>li.active").data("tabid");
            window.parent.SF.tab.addTab('mainmenu' + MenuID, MenuName, MenuURL, true, false, true);
            try {
                window.parent.homeCommon.TaskModal.hide();/*关闭工作台待办弹出框*/
                window.parent.SF.tab.closeTab(p);
            }
            catch (ex) { };
        });
    }
    , CloseFlow: function (saveway) {   //中止流程
        var _this = this;
        $.ajax({
            type: 'Post',
            url: FormUI.curRootPath + '/WfFlowSend/Close',
            data: {
                FlowApplyId: _this.FlowApplyID,
                FlowApplyStepId: _this.FlowApplyStepID,
                Remark: encodeURIComponent($('#FlowRemark').val())
            },
            success: function (result) {
                if (result.flag == 1) {
                    _this.SaveModuleData(saveway, result.data);  //保存模块数据(流程结束)
                    //_this.ClosePage(saveway, result.data.MenuID);
                    return;
                }
                else {
                    layer.msg('中止失败');
                    return;
                }
            }
        });
    }
    , FlowValidDraft: function (e) {
        if ($("#PaperNO").val().length > 50) {
            layer.msg('流程编号不能超过50个字符！！！');
            //alert("流程编号不能超过50个字符！！！");
            $("#PaperNO").focus();
            $(e).attr("disabled", false);
            return false;
        }
        if ($("#FlowApplyName").val().length > 100) {
            layer.msg('流程标题不能超过100个字符！！！');
            $("#FlowApplyName").focus();
            $(e).attr("disabled", false);
            return false;
        }

        return true;
    }
    , FlowValid: function (e) {   //验证
        var _this = this;
        if (_this.FlowStatus == 1) {   //流程发送
            if ($("#PaperNO").val() == '') {
                layer.msg('流程编号不能为空！！！');
                $("#PaperNO").focus();
                $(e).attr("disabled", false);
                return false;
            }
            if ($("#PaperNO").val().length > 50) {
                layer.msg('流程编号不能超过50个字符！！！');
                $("#PaperNO").focus();
                $(e).attr("disabled", false);
                return false;
            }
            if ($("#FlowApplyName").val() == '') {
                layer.msg('流程标题不能为空！！！');
                $("#FlowApplyName").focus();
                $(e).attr("disabled", false);
                return false;
            }
            if ($("#FlowApplyName").val().length > 100) {
                layer.msg('流程标题不能超过100个字符！！！');
                $("#FlowApplyName").focus();
                $(e).attr("disabled", false);
                return false;
            }
        }

        if ($("#divFlowResult").is(":visible")) {  //处理结果
            if ($("input[type='radio'][name='FlowResult']:checked").val() != "-1" && $("input[type='radio'][name='FlowResult']:checked").val() != "1") {
                layer.msg('请选择流程处理结果!');
                $(e).attr("disabled", false);
                IsSaving = false;
                return false;
            }
        }

        if ($("#divFlowStep").is(":visible")) {   //下一步处理事件
            if ($("#FlowStep").val() == "" || $("#FlowStep").val() == null) {
                layer.msg('请选择流程处理事件!');
                $(e).attr("disabled", false);
                IsSaving = false;
                return false;
            }
        }

        if ($("#divFlowUser").is(":visible")) {
            if ($("#FlowUserCode").val() == "") {
                layer.msg('请选择流程处理人员!');
                $(e).attr("disabled", false);
                IsSaving = false;
                return false;
            }
        }

        _this.isSubmit = true;
        if (typeof (SubmitValidFunction) != 'undefined' && !SubmitValidFunction()) {   //提交js验证
            _this.isSubmit = false;
            $(e).attr("disabled", false);
            IsSaving = false;
            return false;
        }



        //其他模块判断是否允许通过
        if (_this.FlowModel == 1 || (_this.FlowModel == 2 && $("input[name='FlowResult']:checked").val() == 1)) {
            var isallowflowend = true;
            var flowResult = $("input[name='FlowResult']:checked").val();
            $("iframe[id^='Sfpluginiframe']").each(function (i, x) {
                if ($.isFunction($(x)[0].contentWindow.IsAllowFlowEnd)) {   //判断项目中是否有允许通过的函数
                    var backData = $(x)[0].contentWindow.IsAllowFlowEnd(_this.FlowApplyID, flowResult);  //执行相关函数
                    if (!backData.IsSuccess)   //项目管理不允许通过
                    {
                        isallowflowend = false;
                        if (backData.Data) {
                            try {
                                var menuid = $(window.parent.document).find('.framecontent [src^="/WfFlowTodo/Index"]').attr("id").replace(/ifrmainmenu/, '');  //待办
                                if (menuid) {
                                    _this.ClosePage(0, menuid, backData.Msg);
                                }
                                else {
                                    layer.msg(backData.Msg);
                                    _this.CloseCurrenPage();
                                }
                            }
                            catch (e) {
                                layer.msg(backData.Msg);
                            }
                        }
                        else {
                            layer.msg(backData.Msg);
                        }
                        return false;
                    }
                }
            });

            if (!isallowflowend) {
                $(e).attr("disabled", false);
                IsSaving = false;
                return false;
            }
        }

        $("input[sfplugins='htmlarea']").each(function (i, x) {
            var editor = UE.getEditor($(x).attr("relId"));
            $(x).val(editor.getContent());
        });

        $("iframe[sftype='office']").each(function (i, x) {//调用office文档保存方法
            isSoffice = true;
            var obj = null;
            var id = $(this).attr("id");
            if (!document.all) {
                obj = document.getElementById(id).contentWindow;
            }
            else {
                obj = document.frames[name];
            }
            obj.TANGER_OCX_SaveEditToServerDatabase();
        });

        return true;
    }
    , FlowControlSetValue: function () {
        $("input[sfplugins='htmlarea']").each(function (i, x) {
            var editor = UE.getEditor($(x).attr("relId"));
            $(x).val(editor.getContent());
        });

        $("iframe[sftype='office']").each(function (i, x) {//调用office文档保存方法
            isSoffice = true;
            var obj = null;
            var id = $(this).attr("id");
            if (!document.all) {
                obj = document.getElementById(id).contentWindow;
            }
            else {
                obj = document.frames[name];
            }
            obj.TANGER_OCX_SaveEditToServerDatabase();
        });
    }
    , GetIncreaseInfo: function () {   //获取自增行数据
        var _this = this;
        var Increase = [];
        $("[sfplugins='increasetable']").each(function (i, x) {                   //自增表格
            var Increasetr = [], Increaseid = [];
            var id = $(x).attr("id");
            $("tr[id=" + id + "]>td").find("[sfplugins]").each(function (j, y) { //自增表格模板列
                var pluginsid = $(y).attr("id");   //模板控件ID
                Increaseid.push(pluginsid);        //控件ID
                var Increasetd = [];               //控件值
                $("tr[id^=" + id + "Sub]>td").find("[id^=" + pluginsid + "Item][sfplugins-item]").each(function (k, z) {                     //自增表格明细项列
                    var itemid = $(z).attr("id");
                    var sfplugins = $(z).attr("sfplugins-item");
                    var itemvalue = "";

                    if (sfplugins == "upload") {
                        itemvalue = $("#" + itemid + "filePath").val();
                    }
                    else if (sfplugins == "image") {
                        itemvalue = $("#" + itemid + "imagePath").val();
                    }
                    else if (sfplugins == "radios") {
                        itemvalue = $("input[name=" + itemid + "]:checked").val();
                    }
                    else if (sfplugins == "checkboxs") {
                        $("input[name=" + itemid + "]:checked").each(function (l, u) {

                            itemvalue = itemvalue + $(u).val() + ",";
                        });
                    }
                        //else if (sfplugins == "htmlarea")
                        //{

                        //}
                    else {
                        itemvalue = $("#" + itemid).val();
                    }

                    Increasetd.push(itemvalue);
                });
                Increasetr.push({ TdId: pluginsid, Increasetd: Increasetd });
            });
            var html = "";
            $("tr[id^=" + id + "Sub]").each(function (j, y) { //自增表格明细项列
                html = html + $(y).prop("outerHTML");   //获取HTML
            });
            html = html + $(x).prop("outerHTML");
            Increase.push({ TrId: id, Increasetr: Increasetr, HTML: encodeURIComponent(html) });
        });
        return Increase;
    }
    , DeleteDraft: function (saveway) {  //删除草稿
        var _this = this;
        $.ajax({
            type: 'Post',
            url: FormUI.curRootPath + "/WfFlowSend/DeletDraft",
            data: { FlowApplyID: _this.FlowApplyID },
            success: function (result) {
                if (result.flag == 1) {
                    _this.ClosePage(saveway, result.data);
                    return;
                }
                else {
                    layer.msg(result.msg);
                    return;
                }
            }
        });
    }
    , DownloadOffice: function (TempId, FlowApplyId, ObjectId) {  //下载Office控件文件        
        $.ajax({
            type: 'Get',
            url: FormUI.curRootPath + '/WfDocTemplate/GetIsDownload',
            data: {
                TempId: TempId,
                FlowApplyId: FlowApplyId,
                ObjectId: ObjectId
            },
            success: function (result) {
                if (result.flag == 1) {
                    window.location.href = FormUI.curRootPath + '/WfDocTemplate/Download?TempId=' + TempId
                        + '&FlowApplyId=' + FlowApplyId
                        + '&ObjectId=' + ObjectId;  //下载文件
                }
                else {
                    layer.msg(result.msg);
                    //alert(result.msg);
                    return;
                }
            }
        });

    }
    , AlertFrame: function (kind) {
        var arrMsg = ['该流程已经被删除、中止或撤回，请处理其他流程！', '您没有查看该流程的权限或流程已被删除！'];
        layer.open({
            content: arrMsg[kind],
            btn: ['确定'],
            icon: 2,
            yes: function (index) {
                layer.close(index);
                FormUI.CloseCurrenPage();
            },
            cancel: function (index) {
                layer.close(index);
                FormUI.CloseCurrenPage();
            }
        });
    }
};