var endDate;
var startDate;
var areaName;
var sourceName;
var allArea;
var allSource;
var max1;
var max2;
var min1;
var min2;
var hiRange;
var loRange;
var alrmFault;
var searchQuery;

$(document).ready(function () {

    //$(".zoneSelection").off("change").on("change", function () {
    //    sourceName = $(".sourceSelection").val();
    //});
    //$("#allArea").off("change").on("change", function () {
    //    allArea = $().val();
    //});

    $(".startingDate").off("change").on("change", function () {
        startDate = $(".startingDate").val();
    });
    $(".endingDate").off("change").on("change", function () {
        endDate = $(".endingDate").val();
    });
    $("#allSource").off("click").on("click", function () {
        allSource = $(this).is(":checked");
    });
    $("#allArea").off("click").on("click", function () {
        allSource = $(this).is(":checked");
    });

    //$(".zoneSelection").off('change').on('change', function () {
    //    areaName = $(".zoneSelection").val();
    //    GetSources();
       
    //}).trigger('change');
    $('.zoneSelection').off('select2:close').on('select2:close', function (e) {
        areaName = $(".zoneSelection").val();
        GetSources();
    });



    $(".getDetails").off("click").on("click", function () {
        startDate = $(".startingDate").val();
        endDate = $(".endingDate").val();
        sourceName = $(".sourceSelection").val();
        areaName = $(".zoneSelection").val();
        if (startDate === "") {
            startDate = "01/01/1999";
        }
        if (endDate === "") {
            endDate = "01/01/2100";
        }
        allSource = $("#allSource").is(":checked");
        allArea = $("#allArea").is(":checked");
        max1 = $("#max1").is(":checked");
        max2 = $("#max2").is(":checked");
        min1 = $("#min1").is(":checked");
        min2 = $("#min2").is(":checked");
        hiRange = $("#hiRange").is(":checked");
        loRange = $("#loRange").is(":checked");
        alrmFault = $("#alrmFault").is(":checked");
        searchQuery = $("#searchQuery").val();

        //allSource = $('input[name="allSource"]').is(':checked');
        
        GetEventDetails();
    });




});

var GetSources = function () {
    $.ajax({
        type: "GET",
        url: "/Home/GetSource?AreaName=" + areaName + "&requestDate=" + Date(),
        contentType: "aplication/json: charset=utf-8",
        dataType: "json",
        success: function (data) {

            var dt = $(".sourceSelection").select2();
            dt.empty();
            var otable = $(".sourceSelection").select2({
                placeholder: "Source Seçimi",
                "data": data.Sources,
                allowClear: true
            });
            //$('.zoneSelection').val(areaName).trigger("change");
            $(".zoneSelection").trigger('update');
        }
    });
};


var GetEventDetails = function () {
    var dt = $("#eventDetails").DataTable();
    dt.destroy();

    var oTable = $("#eventDetails").dataTable({
        dom: 'Bfrtip',
       buttons: ['excel','pdf','print','pageLength'
        ],
        "paging": true,
        "ordering": true,
        "info": true,
        "searching": true,
        "lengthMenu": [[ 50, -1], [ 50, "All"]],
        "serverSide": true,
        "columns": [
            { data: "Bolge", "bSortable": false },
            { data: "Nokta", "bSortable": false },
            { data: "Durum", "bSortable": false },
            { data: "Aciklama", "bSortable": false },
            { data: "BaslamaZamani", "bSortable": false },
            { data: "BitisZamani", "bSortable": false },
            { data: "TimeElapsed", "bSortable": false },
            { data: "Sebep", "bSortable": false }],
        "sAjaxSource": "/Home/GetEventDetail?endDate=" + endDate + "&startDate=" + startDate +
            "&areaName=" + areaName + "&sourceName=" + sourceName + "&allArea=" + allArea + "&allSource=" + allSource +
            "&isMaxOne=" + max1 + "&isMaxTwo=" + max2 + "&isMinOne=" + min1 + "&isMinTwo=" + min2 + "&ishiRange=" + hiRange +
            "&isloRange=" + loRange + "&isalrmFault=" + alrmFault + "&searchQuery=" + searchQuery+ "&requestDate=" + Date(),
        "fnServerData": function (sSource, aoData, fnCallback) {
            $.getJSON(sSource, aoData).done(function (data) {
                fnCallback(data);


            }).fail(function (e) {

            });
        }


    });
};

var RequestToServerJson = function (url, jsonData, operation, successFunction, failFunction) {
    try {
        App.blockUI({ target: "body", boxed: true, zIndex: 9999, message: "yükleniyor" });
        $.post(url, { OperationType: operation, Data: jsonData }, function (data, textStatus, jqXHR) {
            if (data.Status === 1) {
                toastr.success(data.Message, "İşlem");
                successFunction();
            }
            if (data.Status === 2) {
                toastr.error(data.Message, "İşlem");
            }
        }, "json").fail(function (e) {
            failFunction();
        }).always(function () {
            window.setTimeout(function () {
                App.unblockUI();
            }, 1000);
        });
    } catch (e) {
        var mes = e.Message;
    }
};
