function GetCheckedList() {
    var list = [];
    list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
    var cheked = $('input[name="CboxId"]:checked').each(function () {
        list.push({ name: "ids", value: $(this).val() });
    });
    return list;
}

function postNoConfirm(url, param) {
    $.ajax({
        url: url,
        data: param,
        type: "post",
        dataType: "json",
        success: function (result) {
            if (result.Status > 0) {
                window.location.reload();
            }
        },
        error: function (data) { }
    });

    return false;
}

function PostTree() { }

function hiddenLoading() {
    $('#loading').fadeOut("fast");
    //$('#loading').modal('hide');
    $('#loading', window.parent.document).fadeOut("fast");
    $('#loading', window.parent.parent.document).fadeOut("fast");
    //$('#loading', window.parent.document).modal('hide');
    $(".modal-backdrop-Loading").remove();
    $(".modal-backdrop-Loading", window.parent.document).remove();
    $(".modal-backdrop-Loading", window.parent.parent.document).remove();
}

function showHiddenLoading() {
    if ($(".modal-backdrop-Loading").length > 0) {
        return;
    }
    $('<div class="modal-backdrop-Loading"></div>').appendTo(document.body);
    $('#loading', window.parent.document).addClass('in').show();
}

$(function () {
    //菜单点击触发加载遮罩
    $('.postback').change(function ()
    {
        $('<div class="modal-backdrop-Loading"></div>').appendTo(document.body);
        $('#loading', window.parent.document).addClass('in').show();
    });

    $('button').click(function () {
        if (typeof ($(this).attr("dataLoading")) != "undefined") {
            hiddenLoading();
        }
        else if (typeof ($(this).attr("data-toggle")) == "undefined")
        {
            $('<div class="modal-backdrop-Loading"></div>').appendTo(document.body);
            $('#loading', window.parent.document).addClass('in').show();
        }
    });

    //A标签如果加了 class="notLoading" 点击后不显示loading层
    $('a:not(.notLoading)').click(function () {
        if (typeof ($(this).attr("data-toggle")) == "undefined"
            && typeof ($(this).attr("href")) != "undefined"
            && typeof ($(this).attr("target")) == "undefined" 
            && $(this).attr("href") != "" 
            && $(this).attr("href").indexOf("Export") == -1
            && $(this).attr("href").indexOf("Download") == -1
            && $(this).attr("href") != "#" 
            && $(this).attr("href").indexOf("javascript") == -1)
        {
            $('<div class="modal-backdrop-Loading"></div>').appendTo(document.body);
            $('#loading', window.parent.document).addClass('in').show();
        }
    });

   
    hiddenLoading();

    $(".dropSelect").selectpicker(
    {
        liveSearch: true,
        noneSelectedText: "",
        noneResultsText: "未查询到 {0}"
    });

    //修正IE6/7/8不支持数组indexOf
    if (!Array.indexOf) {
        Array.prototype.indexOf = function (obj) {
            for (var i = 0; i < this.length; i++) {
                if (this[i] == obj) {
                    return i;
                }
            }
            return -1;
        }
    }

    $.fn.modal.Constructor.prototype.adjustDialog = function () {
        var modalIsOverflowing = this.$element[0].scrollHeight > document.documentElement.clientHeight

        this.$element.css({
            paddingLeft: !this.bodyIsOverflowing && modalIsOverflowing ? this.scrollbarWidth : '',
            paddingRight: this.bodyIsOverflowing && !modalIsOverflowing ? this.scrollbarWidth : ''
        })

        // 是弹出框居中。。。
        var $modal_dialog = $(this.$element[0]).find(".modal-dialog");
        var m_top = ($(window).height() - $modal_dialog.height()) / 2;
        $modal_dialog.css({ "margin": m_top + "px auto" });
    }

    //修正uploadify的浏览器兼容性
    if ($.fn.uploadify != null) {
        SWFUpload.prototype.cleanUp = function (f) {
            try {
                if (this.movieElement && typeof (f.CallFunction) === "unknown") {
                    this.debug("Removing Flash functions hooks (this should only run in IE and should prevent memory leaks)");
                    for (var h in f) {
                        try {
                            if (typeof (f[h]) === "function" && h[0] >= 'A' && h[0] <= 'Z') {
                                f[h] = null;
                            }
                        } catch (e) {


                        }
                    }
                }
            } catch (g) {

            }

            window.__flash__removeCallback = function (c, b) {
                try {
                    if (c) {
                        c[b] = null;
                    }
                } catch (a) { }
            };
        };
    }

    //全选功能
    $("#CboxAll").click(function () {
        $("input[name='CboxId']").prop("checked", this.checked);
    });

    $('.form_datetime.fdate').datetimepicker(
        {
            format: "yyyy-mm-dd",
            todayBtn: true,
            pickerPosition: "bottom-left"
        });

    $('.form_datetime.fdatetime').datetimepicker(
        {
            format: "yyyy-mm-dd hh:ii:00",
            todayBtn: true,
            pickerPosition: "bottom-left",
            startView: 2,
            minView: 0
            //maxView: 5,
        });

    $('.form_datetime.fyearmonth').datetimepicker(
        {
            format: "yyyy-mm",
            todayBtn: true,
            pickerPosition: "bottom-left",
            startView: 4,
            minView: 3,
            maxView: 4
        });

    $('.form_datetime.ftime').datetimepicker(
    {
        format: "hh:ii",
        todayBtn: false,
        pickerPosition: "bottom-left",
        startView: 1,
        minView: 1,
        maxView: 1
    });

    $(".decimal").bind("keyup afterpaste", function () {
        if (isNaN(this.value))
            execCommand('undo');
    });

    $(".int").bind("keyup afterpaste", function () {
        this.value = this.value.replace(/\D/g, '')
        if (this.value.length > 0) {
            this.value = parseInt(this.value);
        }
    });

    $(".postback").change(function () {
        $("form").submit();
    });

    $(".addList").click(function () {
        if ($('input[name="CboxId"]:checked').length > 0) {
            $(this).attr("disabled", "disabled");

            if (typeof ($(this).attr("confirm")) != "undefined") {
                if (confirm($(this).attr("confirm")) == false) {
                    return false;
                }
            }

            var list = [];
            list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
            var cheked = $('input[name="CboxId"]:checked').each(function () {
                list.push({ name: "ids", value: $(this).val() });
            });
            $.post($(this).attr("href"), list, function (result) {
                if (result.Message != "") {
                    alert(result.Message);
                }
                if (result.Status > 0) {
                    if (result.ReturnUrl == "") {
                        window.location.reload();
                        $(":submit").removeAttr("disabled");
                    }
                    else {
                        window.location.href = result.ReturnUrl;
                    }
                }
                else {
                    $(":submit").removeAttr("disabled");
                    hiddenLoading();
                }
            }, "json").error(function (xhr, errorText, errorType) {
                alert("错误：" + xhr.responseText);
                $(":submit").removeAttr("disabled");
                hiddenLoading();
            });
        }
        else {
            alert("请先勾选需要操作的项目!");
            hiddenLoading();
        }

        return false;
    });

    $(".postNoConfirm").click(function (url, data) {
        $.post(url, data, function (result) {
            if (result.Status > 0) {
            }
            else {

            }
        }, "json").error(function (xhr, errorText, errorType) {
            alert("错误：" + xhr.responseText);
        });

        return false;
    });

    $(".post").on("click", function ()
    {
        PostTree();
        $("form").valid();
        if ($(".input-validation-error").length == 0) {
            if (typeof ($(this).attr("confirm")) != "undefined") {
                if (confirm($(this).attr("confirm")) == false) {
                    hiddenLoading();
                    return false;
                }
            }
            $.post($(this).attr("href"), $("form").serialize(), function (result) {
                if (result.Message != "" && result.Message != undefined) {
                    alert(result.Message);
                }
                if (result.Status > 0 && result.IsRefresh)
                {
                    if (result.ReturnUrl == "")
                    {
                        $(":submit").removeAttr("disabled");
                        window.location.reload();
                    }
                    else
                    {
                        window.location.href = result.ReturnUrl;
                    }
                }
                else
                {
                    hiddenLoading();
                }
            }, "json").error(function (xhr, errorText, errorType) {
                alert("错误：" + xhr.responseText);
                hiddenLoading();
            });
        }
        else {
            hiddenLoading();
        }

        return false;
    });

    $(".postList").click(function () {
        if (typeof ($(this).attr("confirm")) != "undefined") {
            if (confirm($(this).attr("confirm")) == false) {
                hiddenLoading();
                return false;
            }
        }

        var isNofresh = $(this).hasClass("NoRefresh");
        if ($('input[name="CboxId"]:checked').length > 0) {
            $(this).attr("disabled", "disabled");
            var obj = $(this);
            var list = [];
            list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
            var cheked = $('input[name="CboxId"]:checked').each(function () {
                list.push({ name: "ids", value: $(this).val() });
            });
            $.post($(this).attr("href"), list, function (result) {
                if (result.Message != "" && result.Message != undefined) {
                    alert(result.Message);
                }

                if (result.Status > 0) {
                    if (isNofresh == false)
                    {
                        if (result.ReturnUrl == "")
                        {
                            window.location.reload();
                        }
                        else
                        {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else
                    {                        
                        obj.removeAttr("disabled");
                        hiddenLoading();
                    }
                }
                else {
                    obj.removeAttr("disabled");
                    hiddenLoading();
                }
            }, "json").error(function (xhr, errorText, errorType) {
                alert("错误：" + xhr.responseText);
                obj.removeAttr("disabled");
                hiddenLoading();
            });
        }
        else {
            alert("请先勾选需要操作的项目!");
            hiddenLoading();
        }

        return false;
    });

    $(".Check").click(function () {
        if ($('input[name="CboxId"]:checked').length > 0) {
            if (confirm("确定要通过所选?")) {
                $(this).attr("disabled", "disabled");
                var list = [];
                list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
                var cheked = $('input[name="CboxId"]:checked').each(function () {
                    list.push({ name: "ids", value: $(this).val() });
                });
                list.push({ name: "Value", value: 1 });
                $.post($(this).attr("href"), list, function (result) {
                    if (result.Message != "") {
                        alert(result.Message);
                    }

                    if (result.Status > 0) {
                        if (result.ReturnUrl == "") {
                            window.location.reload();
                            $(":submit").removeAttr("disabled");
                        }
                        else {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else {
                        $(":submit").removeAttr("disabled");
                    }
                }, "json").error(function (xhr, errorText, errorType) {
                    alert("错误：" + xhr.responseText);
                    $(":submit").removeAttr("disabled");
                });
            }
        }
        else {
            alert("请先勾选需要通过的项目!");
            hiddenLoading();
        }

        return false;
    });

    $(".UnCheck").click(function () {
        if ($('input[name="CboxId"]:checked').length > 0) {
            if (confirm("确定要不通过所选?")) {
                $(this).attr("disabled", "disabled");
                var list = [];
                list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
                var cheked = $('input[name="CboxId"]:checked').each(function () {
                    list.push({ name: "ids", value: $(this).val() });
                });
                list.push({ name: "Value", value: -1 });
                $.post($(this).attr("href"), list, function (result) {
                    if (result.Message != "") {
                        alert(result.Message);
                    }

                    if (result.Status > 0) {
                        if (result.ReturnUrl == "") {
                            window.location.reload();
                            $(":submit").removeAttr("disabled");
                        }
                        else {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else {
                        $(":submit").removeAttr("disabled");
                    }
                }, "json").error(function (xhr, errorText, errorType) {
                    alert("错误：" + xhr.responseText);
                    $(":submit").removeAttr("disabled");
                });
            } else {
                hiddenLoading();
            }
        }
        else {
            alert("请先勾选需要不通过的项目!");
            hiddenLoading();
        }

        return false;
    });

    $(".delete").click(function () {
        if ($('input[name="CboxId"]:checked').length > 0) {
            if (confirm("确定要删除所选?")) {
                $(this).attr("disabled", "disabled");
                var list = [];
                list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
                var cheked = $('input[name="CboxId"]:checked').each(function () {
                    list.push({ name: "ids", value: $(this).val() });
                });
                $.post($(this).attr("href"), list, function (result) {
                    if (result.Message != "") {
                        alert(result.Message);
                    }

                    if (result.Status > 0) {
                        if (result.ReturnUrl == "") {
                            window.location.reload();
                            $(":submit").removeAttr("disabled");
                        }
                        else {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else {
                        $(":submit").removeAttr("disabled");
                        hiddenLoading();
                    }
                }, "json").error(function (xhr, errorText, errorType) {
                    alert("错误：" + xhr.responseText);
                    $(":submit").removeAttr("disabled");
                    hiddenLoading();
                });
            } else {
                hiddenLoading();
            }
        }
        else {
            alert("请先勾选需要删除的项目!");
            hiddenLoading();
        }

        return false;
    });


    $(".lcRetrySms").click(function () {
        if ($('input[name="CboxId"]:checked').length > 0) {
            if (confirm("确定要重新发送所选短信?")) {
                $(this).attr("disabled", "disabled");
                var list = [];
                list.push({ name: "__RequestVerificationToken", value: $('input[name="__RequestVerificationToken"]').val() });
                var cheked = $('input[name="CboxId"]:checked').each(function () {
                    list.push({ name: "ids", value: $(this).val() });
                });
                $.post($(this).attr("href"), list, function (result) {
                    if (result.Message != "") {
                        alert(result.Message);
                    }

                    if (result.Status > 0) {
                        if (result.ReturnUrl == "") {
                            window.location.reload();
                            $(":submit").removeAttr("disabled");
                        }
                        else {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else {
                        $(":submit").removeAttr("disabled");
                        hiddenLoading();
                    }
                }, "json").error(function (xhr, errorText, errorType) {
                    alert("错误：" + xhr.responseText);
                    $(":submit").removeAttr("disabled");
                    hiddenLoading();
                });
            } else {
                hiddenLoading();
            }
        }
        else {
            alert("请先勾选需要重新发送的短信!");
            hiddenLoading();
        }

        return false;
    });

    $("form").submit(function ()
    {
        PostTree();
        $(":submit", this).attr("disabled", "disabled");
        var isdialog = $(":submit", this).parents(".form-group").hasClass("hidden");
        var isNofresh = $(":submit", this).hasClass("NoRefresh");
        var wechat = $("form").attr("wechat");
        //$("form").valid();
        if ($(".input-validation-error").length == 0) {
            if ($("form").attr("enctype") != null && $("form").attr("enctype").indexOf("multipart") > -1) {
                return true;
            }
            else
            {
                $.post($("form").attr("action"), $("form").serialize(), function (result)
                {
                    if (result.Message != "" && result.Message != undefined)
                    {
                        CustomAlert(wechat, result.Message);
                    }

                    if (result.Status > 0)
                    {
                        if (isdialog)
                        {
                            if (result.Message == "" || result.Message == undefined)
                            {
                                CustomAlert(wechat, "操作成功!");
                            }
                            $(".close", window.parent.document).click();
                            if (isNofresh == false) {
                                if (result.ReturnUrl == "") {
                                    window.parent.location.reload();
                                }
                                else {
                                    window.parent.location.href = result.ReturnUrl;
                                }
                            }

                            return false;
                        }

                        if (result.ReturnUrl == "") {
                            $(":submit").removeAttr("disabled");
                            hiddenLoading();
                        }
                        else {
                            window.location.href = result.ReturnUrl;
                        }
                    }
                    else {
                        $(":submit").removeAttr("disabled");
                        hiddenLoading();
                    }
                }, "json").error(function (xhr, errorText, errorType)
                {
                    //alert("错误：" + xhr.responseText);
                    CustomAlert(wechat,  xhr.responseText);
                    $(":submit").removeAttr("disabled");
                    hiddenLoading();
                });
            }
        }
        else {
            $(":submit").removeAttr("disabled");
            hiddenLoading();
        }

        return false;
    });

    function CustomAlert(wechat, msg) {
        if (wechat == "true") {
            mui.alert(msg);
        }
        else {
            alert(msg);
        }
    }

    //$("form").submit(function () {
    //    $(":submit", this).attr("disabled", "disabled");
    //    mui.ajax($("form").attr("action"), {
    //        data: $("form").serialize(),
    //        dataType: 'json',
    //        type: 'post',
    //        timeout: 10000,
    //        headers: { 'Content-Type': 'application/json' },
    //        success: function (result) {
    //            if (result.Message != "" && result.Message != undefined) {
    //                mui.alert(result.Message);
    //            }

    //            if (data.Status > 0) {
    //                if (result.ReturnUrl == "") {
    //                    $(":submit").removeAttr("disabled");
    //                }
    //                else {
    //                    window.location.href = result.ReturnUrl;
    //                }
    //            }
    //            else
    //            {
    //                $(":submit").removeAttr("disabled");
    //            }
    //        },
    //        error: function (xhr, type, errorThrown) {
    //            mui.alert(xhr.responseText);
    //            $(":submit").removeAttr("disabled");
    //        }
    //    });
    //})

});