$(function () {

    $('#ExchangeButton').click(function () {
        //alert("clickEx");
        $.ajax({
            method: 'POST',
            url: '/Point/ExchangePoint',
            data: {
                AppId: $('#ExchangeAppID').val(),
                ExPoint: $("#ExchangePoint").val(),
            },
            success: function (data) {
                if (data.Code == '0001') {
                    alert(data.Message);
                } else {
                    alert(data.Message);
                    location.reload();
                }
            },
        })
    });










})