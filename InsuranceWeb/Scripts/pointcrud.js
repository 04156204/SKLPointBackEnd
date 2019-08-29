$(function () {

    $("#input-point").click(function () {
        //alert("click");
        $.ajax({
            url: '/Point/InsertPoint',
            method: 'POST',
            data: $("#insert-point-form").serialize(),
            success: function (reJson) {
                if (reJson.Code == '0001') {
                    alert(reJson.Message);
                }
                else {
                    alert(reJson.Message + "為您將該位客戶點數資料顯示在下方！");
                    $.ajax({
                        url: '/Point/BothSearch',
                        method: 'POST',
                        data: $("#insert-point-form").serialize(),
                        success: function (reJson) {
                            $("#detailData").html(reJson)
                            document.getElementById("insert-point-form").reset();
                        },
                    })
                }
            },
        })
    });


    $('#SearchButton').click(function () {//用id跟class的差別 id是唯一
        //alert("click~~");
        $.ajax({
            method: 'POST',
            url: '/Point/Search',
            data: {
                AppId: $('#Search_Id').val(),
                Search_AcType: $("#Search_AcType").val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            },
            success: function (data) {
                if (data.Code == '0001') {
                    alert(data.Message);
                } else {
                    //location.reload();
                    $("#detailData").html(data)
                }
            },
        })
    });

    $('#SearchLogButton').click(function () {
        //alert("click~~");
        $.ajax({
            method: 'POST',
            url: '/Point/SearchLog',
            data: {
                AppId: $('#SearchLog_Id').val(),
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val(),
            },
            success: function (data) {
                if (data.Code == '0001') {
                    alert(data.Message);
                } else {
                    $("#logDetailData").html(data)
                }
            },
        })
    });





    $(document).on("click", '.btnCancel', function (e) {
        var $this = $(this);
        //alert("clickCancel");
        $.ajax({
            method: 'POST',
            url: '/Point/CancelPoint',
            data: {
                id: $this.data('id'),
            },
            success: function (reJson) {
                alert(reJson.Message);
                location.reload();
            },
        })
    });






})