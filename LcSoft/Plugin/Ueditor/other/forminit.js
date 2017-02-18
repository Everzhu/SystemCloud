; var formeditor = {
    SfEditor: null,      //编辑器
    FormName: "",       //表单名称
    FormVID: 0,        //表单版本编号
    FormTypeID: 0,     //表单类别编号
    FormTypeName: "",    //表单类别名称
    FormRemark: "",     //备注
    FormHtml: "",        //表单HTML
    CurrentPath: "",
    init: function () {
        var _this = this;
        if ($("#FormVID").val()) {
            _this.FormVID = $("#FormVID").val();
        }

        if (_this.FormVID != 0) {
            _this.getFormData(_this.FormVID);
        }
        else {
            _this.initEditor("myFormDesign");
        }

        _this.save();    //提交
        _this.review();  //预览
        _this.imports(); //导入
        _this.exports(); //导出                 
    },
    initEditor: function (id) {
        var _this = this;
        _this.SfEditor = UE.getEditor(id, {
            newId: 'sf',
            toolsf: true,//是否显示，设计器的 toolbars
            textarea: 'design_content',
            //focus时自动清空初始化时的内容f
            //autoClearinitialContent:true,
            //关闭字数统计
            wordCount: false,
            //关闭elementPath
            elementPathEnabled: false,
            //默认的编辑区域高度
            initialFrameHeight: document.body.clientHeight - 151,
            initialFrameWidth: ""
            //iframeCssUrl: "bootstrap.css" //引入自身 css使编辑器兼容你网站css
            //更多其他参数，请参考ueditor.config.js中的配置项)
        });
        _this.readonltext();
    },
    exec: function (sfplugins) {  //添加控件
        var _this = this;
        _this.SfEditor.execCommand(sfplugins);
    },
    review: function () {
        var _this = this;
        $("#btnReview").click(function () {
            $("#htmlContent").html(_this.SfEditor.getContent().replace(/<table/g, '<table class="tablestyle" border="1px"'));
            $("#myModal").modal('show');
            FormUI.init(0);   //预览
            return;
        });
    },
    imports: function () {
        var _this = this;
        $("#btnImport").click(function () {
            _this.exec('import');
        });
    },
    exports: function () {
        var _this = this;
        $("#btnExport").click(function () {
            $.ajax({
                type: 'Post',
                url: _this.CurrentPath + '/WfFormDefine/SaveExport',
                data: {
                    'HtmlContent': encodeURIComponent(_this.SfEditor.getContent())
                },
                success: function (result) {
                    if (result.flag == 1) {
                        window.location.href = _this.CurrentPath + "/WfFormDefine/Export?FormName=" + encodeURIComponent($("#FormName").val());
                    }
                    else {
                        layer.msg('导出失败！');
                        return;
                    }
                }
            });
        });
    },
    getQueryStringByName: function (name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    },
    save: function () {
        var _this = this;
        $("#btnCommit").click(function () {
            _this.saveForm();
        });
    },
    saveForm: function () {
        var _this = this;
        if (!$("#FormName").val().trim()) {
            layer.msg('请输入表单名称');
            return false;
        }
        if (!$("#FormType").val()) {
            layer.msg('请选择表单类别!');
            return false;
        }
        if (_this.FormVID != "" && _this.FormVID != "0" && $(parent.document).find("div.breadnav-content ul li[data-tabid='WfDefinitionEditor']").length > 0)   //流程定义-新建流程 页面打开着不能编辑表单保存
        {
            layer.alert('您正在新建或编辑流程，修改的表单可能导致流程关联的表单有误，请先关闭新建流程页面,再保存表单！', { icon: 5 });
            return false;
        }
        if (_this.SfEditor.queryCommandState('source')) {
            _this.SfEditor.execCommand('source');//切换到编辑模式才提交，否则有bug
        }

        if (_this.SfEditor.getContent().length > 0) {   //表单内容存在
            _this.FormName = $("#FormName").val();
            _this.FormTypeName = $("#FormType option:selected").text();
            _this.FormTypeID = $("#FormType").val();
            _this.FormRemark = $("#FormRemark").val();
            _this.FormHtml = _this.SfEditor.getContent().replace(/<table/g, '<table class="tablestyle" border="1px"');
            var FormAttrInfo = _this.getPluginsAttr(_this.SfEditor.document);
            var FormAttr = FormAttrInfo[0];
            var FormFieldInfo = FormAttrInfo[1];

            $.ajax({
                type: "Post",
                url: _this.CurrentPath + "/WfFormDefine/Save",
                data: {
                    'FormVID': _this.FormVID,
                    'FormName': _this.FormName,
                    'FormTypeID': _this.FormTypeID,
                    'FormTypeName': _this.FormTypeName,
                    'FormHtml': encodeURIComponent(_this.FormHtml),
                    'FormAttr': FormAttr,
                    'FormFieldInfo': FormFieldInfo,
                    'FormRemark': _this.FormRemark
                },
                success: function (data) {
                    if (data.flag == 1) {
                        layer.confirm('保存成功！', { icon: 1, title: '信息', btn: ['确定'], closeBtn: 0 }, function (index) {
                            layer.close(index);
                            var MenuId = _this.getQueryStringByName("MenuID");
                            var p = $(window.parent.document).find("#tab-box-list>li.active").data("tabid");
                            window.parent.SF.tab.addTab("mainmenu" + MenuId, "表单定义", _this.CurrentPath + "/WfFormDefine/Index", true, true, true);  //打开已办页面
                            window.parent.SF.tab.closeTab(p);
                        });

                        return;
                    }
                    else {
                        layer.msg(data.msg);
                        return;
                    }
                }
            });
        }
        else {
            layer.msg('表单内容不能为空！');
            return false;
        }
    },
    updateForm: function (formversion, formhtml) {  //更新表单的属性
        var _this = this;
        if (formhtml) {
            var FormAttrInfo = _this.getPluginsAttr(formhtml);
            var FormAttr = FormAttrInfo[0];
            $.ajax({
                type: 'Post',
                url: _this.CurrentPath + '/WfFormDefine/UpdateFieldProperty',
                data: { id: formversion.Id, formAttr: FormAttr },
                success: function (result) {
                    if (result.flag == 1) {

                    }
                }
            });
        }
    },
    getPluginsAttr: function (sf) {  //获取控件属性
        var _this = this;
        var array = [], FieldArrary = [];
        var FormField = "", FormFieldName = "", FormFieldTitle = "", FormFieldType = "",
                FormFieldLength = "", FormFieldValue = "", FormFieldItemType = "",
                FormFieldItemList = "", FormFieldProperty = "";
        //var sf = _this.SfEditor.document;
        var sfelement = $(sf).find("[sfplugins]");   //获取控件元素
        var ids = _this.getIncreaseControlId(sf);    //获取自增行控件ID
        var count = sfelement.length;                //获取控件数量
        for (var i = 0; i < count; i++) {
            var obj = sfelement[i];
            if (obj) {
                var sfplugins = $(obj).attr("sfplugins").toLowerCase();
                var fieldname = $(obj).attr("id")
                   , fieldtitle = $(obj).attr("title")
                   , fieldtype = sfplugins
                   , fieldlength = $(obj).attr("orgtextlimit")
                   , fieldvalue = $(obj).val()
                   , itemtype = $(obj).attr("orgtype")
                   , itemlist = $(obj).attr("orgitemlist")
                   , fieldvalid = $(obj).attr("orgisrequire")
                   , isincrease = 0
                   , isbind = 0
                   , bindsource = ""
                   , iscondition = $(obj).attr("orgcondition")
                   , fieldproperty = "";
                FormFieldName = FormFieldName + fieldname + "|";
                FormFieldTitle = FormFieldTitle + fieldtitle + "|";
                FormFieldType = FormFieldType + fieldtype + "|";
                FormFieldLength = FormFieldLength + fieldlength + "|";
                FormFieldValue = FormFieldValue + fieldvalue + "|";
                FormFieldItemType = FormFieldItemType + (itemtype == undefined ? "" : itemtype) + "|";
                FormFieldItemList = FormFieldItemList + (itemlist == undefined ? "" : itemlist) + "|";
                if (ids && ids.indexOf(fieldname) >= 0)   // 自增行内的控件
                {
                    isincrease = 1;
                }
                if (sfplugins == "increasetable") {
                    fieldtitle = "自增行";
                    var orgchild = "";
                    $(obj).find("[sfplugins]").each(function (i, x) {
                        orgchild = orgchild + $(x).attr('id') + ',';
                    });
                    orgchild = orgchild.replace(/,$/g, '');
                    fieldproperty = JSON.stringify({ orgchild: orgchild });
                }  //自增行
                else if (sfplugins == "calculation")   //计算控件
                {
                    var orgitemlist = $(obj).attr('orgitemlist');
                    var orgresultformat = $(obj).attr('orgresultformat');
                    var orgformula = $(obj).attr('orgformula');
                    var orgformulaval = $(obj).attr('orgformulaval');
                    var orgdataformat = $(obj).attr('orgdataformat');
                    var orgdotnumber = $(obj).attr('orgdotnumber');

                    if (orgdataformat == "四舍五入") {
                        orgdataformat = "round";
                    }
                    else if (orgdataformat == "结果加1") {
                        orgdataformat = "ceil";
                    }
                    else if (orgdataformat == "保留小数") {
                        orgdataformat = "decimal";
                    }
                    else if (orgdataformat == "人民币大写") {
                        orgdataformat = "money";
                    }

                    fieldproperty = JSON.stringify({
                        orgitemlist: orgitemlist, orgresultformat: orgresultformat,
                        orgformula: orgformula, orgformulaval: orgformulaval, orgdataformat: orgdataformat,
                        orgdotnumber: orgdotnumber
                    });
                }
                else if (sfplugins == 'statistics')    //统计控件
                {
                    var orgstatisticsitem = $(obj).attr('orgstatisticsitem');
                    var orgmethod = $(obj).attr('orgmethod');
                    var orgdotnumber = $(obj).attr('orgdotnumber');
                    fieldproperty = JSON.stringify({ orgstatisticsitem: orgstatisticsitem, orgmethod: orgmethod, orgdotnumber: orgdotnumber });
                }
                else if (sfplugins == 'text')          //文本框
                {
                    var orgfigure = $(obj).attr('orgfigure');
                    var orgtype = $(obj).attr('orgtype');
                    if (orgtype == 'float' || orgtype == 'text') {
                        fieldproperty = JSON.stringify({
                            orgfigure: orgfigure
                        });
                    }
                    else if (orgtype == 'int' || orgtype == 'zint' || orgtype == 'fint') {
                        var orgrangebegin = $(obj).attr('orgrangebegin');
                        var orgrangeend = $(obj).attr('orgrangeend');
                        fieldproperty = JSON.stringify({
                            orgfigure: orgfigure,
                            orgrangebegin: orgrangebegin, orgrangeend: orgrangeend
                        });
                    }
                }
                else if (sfplugins == 'autonumber')    //自动编号
                {
                    var orgprefix = $(obj).attr('orgprefix');        //前缀
                    var orgdatetype = $(obj).attr('orgdatetype');    //数据格式
                    var orgseparator = $(obj).attr('orgseparator');  //分隔符
                    var orgnumfigures = $(obj).attr('orgnumfigures') //编码位数
                    var orgnumbegin = $(obj).attr('orgnumbegin');    //编号起始
                    if (!orgnumfigures || !$.isNumeric(orgnumfigures)) {
                        orgnumfigures = 0;
                    }
                    if (!orgnumbegin || !$.isNumeric(orgnumbegin)) {
                        orgnumbegin = 0;
                    }

                    fieldproperty = JSON.stringify({
                        orgprefix: orgprefix, orgdatetype: orgdatetype,
                        orgseparator: orgseparator, orgnumfigures: orgnumfigures,
                        orgnumbegin: orgnumbegin
                    });
                }
                else if (sfplugins == 'date')          //日期
                {
                    var orgdatetimetype = $(obj).attr('orgdatetimetype');
                    var orgdatetype = $(obj).attr('orgdatetype');
                    if (orgdatetimetype == "2") {  //发起时间
                        orgdatetype = "yyyy-MM-dd HH:mm";
                    }
                    else {
                        orgdatetimetype = 1;
                    }
                    fieldproperty = JSON.stringify({ orgdatetimetype: orgdatetimetype, orgdatetype: orgdatetype });
                }
                else if (sfplugins == 'macros')        //宏控件
                {
                    var orgtype = $(obj).attr('orgtype');
                    fieldproperty = JSON.stringify({ orgtype: orgtype });
                }
                else if (sfplugins == 'radios') {     //单选项
                    var defaultvalue = $(obj).find('input[checked]')
                    if (defaultvalue && defaultvalue.length > 0) {
                        fieldvalue = defaultvalue.attr('value');
                    }
                }
                else if (sfplugins == 'checkboxs') {  //复选项
                    var defaultvalue = $(obj).find('input[checked]')
                    if (defaultvalue && defaultvalue.length > 0) {
                        fieldvalue = '';
                        defaultvalue.each(function (k, z) {
                            fieldvalue = fieldvalue + $(z).attr('value') + ',';
                        });
                        fieldvalue = fieldvalue.replace(/,$/g, '');
                    }
                }
                var bindarray = _this.getBindInfo(obj);   //获取控件绑定信息
                var FieldAtt = {
                    FieldName: (fieldname ? fieldname : ''),        //编号
                    FieldTitle: (fieldtitle ? fieldtitle : ''),     //名称
                    FieldType: (fieldtype ? fieldtype : ''),        //类型
                    FieldLength: (fieldlength ? fieldlength : ''),  //限制长度
                    FieldValue: (fieldvalue ? fieldvalue : ''),     //默认值
                    ItemType: (itemtype ? itemtype : ''),           //二级类型
                    ItemList: (itemlist ? itemlist : ''),           //子项目
                    FieldValid: (fieldvalid == "true" ? 1 : 0),     //是否必填
                    IsIncrease: isincrease,                         //是否为自增行内控件
                    IsBind: bindarray[0],                           //是否绑定数据
                    BindSource: bindarray[1],                       //绑定数据源
                    IsCondition: (iscondition ? iscondition : 1),   //是否显示
                    FieldProperty: fieldproperty                    //控件其他属性
                };
                array.push(FieldAtt);
            }
        }

        if (FormFieldName != "") {
            FormField = FormFieldName.substr(0, FormFieldName.length - 1) + ";"
                      + FormFieldTitle.substr(0, FormFieldTitle.length - 1) + ";"
                      + FormFieldType.substr(0, FormFieldType.length - 1) + ";"
                      + FormFieldLength.substr(0, FormFieldLength.length - 1) + ";"
                      + FormFieldValue.substr(0, FormFieldValue.length - 1) + ";"
                      + FormFieldItemType.substr(0, FormFieldItemType.length - 1) + ";"
                      + FormFieldItemList.substr(0, FormFieldItemList.length - 1);
        }
        FieldArrary.push(encodeURIComponent(JSON.stringify(array)));
        FieldArrary.push(FormField);
        return FieldArrary;
    },
    getIncreaseControlId: function (sf) {  //获取自增行控件ID
        var ids = [];
        $(sf).find("[sfplugins=increasetable]").each(function (i, x) {
            $(x).find("[sfplugins]").each(function (j, y) {
                ids.push($(y).attr("id"));
            });
        });
        return ids;
    },
    getBindInfo: function (x) {    //获取控件绑定数据信息
        var bindsource = "";
        var isbind = 0;
        var orgbindtype = $(x).attr("orgbindtype");
        if (!orgbindtype || orgbindtype == "fix") {
            isbind = 0;
            bindsource = "";
        }
        else if (orgbindtype == "source") {
            isbind = 1;
            var orgsourcetable = $(x).attr("orgsourcetable");
            var orgsourcetext = $(x).attr("orgsourcetext");
            var orgsourcevalue = $(x).attr("orgsourcevalue");
            bindsource = "{";
            bindsource = bindsource + "'sourcetable':'" + (orgsourcetable ? orgsourcetable : "") + "',";
            bindsource = bindsource + "'sourcetext':'" + (orgsourcetext ? orgsourcetext : "") + "',";
            bindsource = bindsource + "'sourcevalue':'" + (orgsourcevalue ? orgsourcevalue : "") + "'";
            bindsource = bindsource + "}";
        }
        else if (orgbindtype == "tsql") {
            isbind = 2;
            var orgsourcetsql = $(x).attr("orgsourcetsql");
            bindsource = "{";
            bindsource = bindsource + "'sourcetsql':'" + (orgsourcetsql ? orgsourcetsql : "") + "'";
            bindsource = bindsource + "}";
        }
        else if (orgbindtype == "api") {
            isbind = 3;
            var orgapiname = $(x).attr("orgapiname");
            bindsource = "{";
            bindsource = bindsource + "'apiname':'" + (orgapiname ? orgapiname : "") + "'";
            bindsource = bindsource + "}";
        }

        var array = [];
        array.push(isbind);
        array.push(bindsource);
        return array;
    },
    getFormData: function (FormVID) {
        var _this = this;
        $.ajax({
            type: "Get",
            url: _this.CurrentPath + '/WfFormDefine/GetFormHtmlData/' + FormVID,
            dataType: 'json',
            success: function (d) {
                var data = d.data;
                if (d.flag == 0) {
                    if (data != null) {
                        $("#FormType").val(data.FormTypeId).trigger("change");
                        $("#FormName").val(data.FormName);
                        $("#FormRemark").val(data.Remark);
                    }
                    _this.initEditor("myFormDesign");
                    layer.msg(d.msg);
                    return;
                }
                else {

                    $("#myFormDesign").html(data.FormHtml);
                    $("#FormType").val(data.FormTypeId).trigger("change");
                    $("#FormName").val(data.FormName);
                    $("#FormRemark").val(data.Remark);
                    _this.initEditor("myFormDesign");
                    return;
                }

            }

        });
    },
    readonltext: function () {
        var _this = this;
        _this.SfEditor.addListener('afterinserthtml', function (html) {
            var idArray = new Array();
            var _$ = $(document.body).find("#ueditor_sf_0").contents();    //获取编辑的HTML
            _$.find("[sfplugins][id]").each(function (i, x) {
                var id = $(x).attr("id");
                var sfplugins = $(x).attr("sfplugins");
                if ($.inArray(id, idArray) >= 0)   //存在重复的ID
                {
                    var oldid = id;
                    id = id + "repeat" + i;   //新ID
                    $(x).attr("id", id);
                    $(x).attr("name", id);
                    if (sfplugins == "radios") {
                        _$.find("span[id=" + id + "]>input[name=" + oldid + "][type=radio]").attr("name", id);
                    }
                    else if (sfplugins == "checkboxs") {
                        _$.find("span[id=" + id + "]>input[name=" + oldid + "][type=checkbox]").attr("name", id);
                    }
                }
                idArray.push(id);   //将当前id插入数组
            });

        });
    }
};