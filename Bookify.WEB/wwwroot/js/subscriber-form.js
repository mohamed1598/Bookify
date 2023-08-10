$(document).ready(function () {
    $('#GovernerateId').on('change', function () {
        var GovernerateId = $(this).val();
        var areaList = $('#AreaId');

        areaList.empty();
        areaList.append('<option></option>');

        if (GovernerateId !== '') {
            $.ajax({
                url: '/Subscribers/GetAreas?GovernerateId=' + GovernerateId,
                success: function (areas) {
                    $.each(areas, function (i, area) {
                        var item = $('<option></option>').attr("value", area.value).text(area.text);
                        areaList.append(item);
                    });
                },
                error: function () {
                    showErrorMessage();
                }
            });
        }
    });
});