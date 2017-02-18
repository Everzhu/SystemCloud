/** 
    * desc : 合并指定表格（表格id为table_id）指定列（列数为table_colnum）的相同文本的相邻单元格   
    * @table_id 表格id : 为需要进行合并单元格的表格的id。如在HTMl中指定表格 id="data" ，此参数应为 #data    
    * @table_colnum   : 为需要合并单元格的所在列.参考jQuery中nth-child的参数.若为数字，从最左边第一列为1开始算起;"even" 表示偶数列;"odd" 表示奇数列; "3n+1" 表示的列数为1、4、7、......  
    * @table_minrow ? : 可选的,表示要合并列的行数最小的列,省略表示从第0行开始 (闭区间)  
    * @table_maxrow ? : 可选的,表示要合并列的行数最大的列,省略表示最大行列数为表格最后一行 (开区间)  
    */
function table_rowspan(table_id, table_colnum)
{
    if (table_colnum == "even")
    {
        table_colnum = "2n";
    }
    else if (table_colnum == "odd")
    {
        table_colnum = "2n+1";
    }
    else
    {
        table_colnum = "" + table_colnum;
    }
    var cols = [];
    var all_row_num = $(table_id + " tr td:nth-child(1)").length;
    var all_col_num = $(table_id + " tr:nth-child(1)").children().length;
    if (table_colnum.indexOf("n") == -1)
    {
        cols[0] = table_colnum;
    }
    else
    {
        var n = 0;
        var a = table_colnum.substring(0, table_colnum.indexOf("n"));
        var b = table_colnum.substring(table_colnum.indexOf("n") + 1);
        //alert("a="+a+"b="+(b==true));  
        a = a ? parseInt(a) : 1;
        b = b ? parseInt(b) : 0;
        //alert(b);  
        while (a * n + b <= all_col_num)
        {
            cols[n] = a * n + b;
            n++;
        }
    }
    var table_minrow = arguments[2] ? arguments[2] : 0;
    var table_maxrow = arguments[3] ? arguments[3] : all_row_num + 1;
    var table_firsttd = "";
    var table_currenttd = "";
    var table_SpanNum = 0;
    for (var j = 0; j < cols.length; j++)
    {
        $(table_id + " tr td:nth-child(" + cols[j] + ")").slice(table_minrow, table_maxrow).each(function (i)
        {
            var table_col_obj = $(this);
            if (table_col_obj.html() != " ")
            {
                if (i == 0)
                {
                    table_firsttd = $(this);
                    table_SpanNum = 1;
                }
                else
                {
                    table_currenttd = $(this);
                    if (table_firsttd.text() == table_currenttd.text())
                    {
                        table_SpanNum++;
                        table_currenttd.hide(); //remove();    
                        table_firsttd.attr("rowSpan", table_SpanNum);
                    } else
                    {
                        table_firsttd = $(this);
                        table_SpanNum = 1;
                    }
                }
            }
        });
    }
}

/** 
* desc : 合并指定表格（表格id为table_id）指定行（行数为table_rownum）的相同文本的相邻单元格     
* @table_id 表格id : 为需要进行合并单元格的表格的id。如在HTMl中指定表格 id="data" ，此参数应为 #data    
* @table_rownum   : 为需要合并单元格的所在行.参考jQuery中nth-child的参数.若为数字，从最左边第一列为1开始算起;"even" 表示偶数行;"odd" 表示奇数行; "3n+1" 表示的行数为1、4、7、......  
* @table_mincolnum ? : 可选的,表示要合并行中的最小列,省略表示从第0列开始(闭区间) 
* @table_maxcolnum ? : 可选的,表示要合并行中的最大列,省略表示表格的最大列数(开区间)  
*/
function table_colspan(table_id, table_rownum)
{
    //if(table_maxcolnum == void 0){table_maxcolnum=0;}  
    var table_mincolnum = arguments[2] ? arguments[2] : 0;
    var table_maxcolnum;
    var table_firsttd = "";
    var table_currenttd = "";
    var table_SpanNum = 0;
    $(table_id + " tr:nth-child(" + table_rownum + ")").each(function (i)
    {
        table_row_obj = $(this).children();
        table_maxcolnum = arguments[3] ? arguments[3] : table_row_obj.length;
        table_row_obj.slice(table_mincolnum, table_maxcolnum).each(function (i)
        {
            if (i == 0)
            {
                table_firsttd = $(this);
                table_SpanNum = 1;
            } else if ((table_maxcolnum > 0) && (i > table_maxcolnum))
            {
                return "";
            } else
            {
                table_currenttd = $(this);
                if (table_firsttd.text() == table_currenttd.text())
                {
                    table_SpanNum++;
                    if (table_currenttd.is(":visible"))
                    {
                        table_firsttd.width(parseInt(table_firsttd.width()) + parseInt(table_currenttd.width()));
                    }
                    table_currenttd.hide(); //remove();    
                    table_firsttd.attr("colSpan", table_SpanNum);
                } else
                {
                    table_firsttd = $(this);
                    table_SpanNum = 1;
                }
            }
        });
    });
}
