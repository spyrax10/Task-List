$(document).ready(function () {

    chkTab();

    // listening to click event of each checkboxes
    $('.chkId').click(function () {

        var Id = $(this).val();

        if ($(this).is(":checked")) {
            $(this).closest("tr").addClass("marked");
          
            updateStat(Id, "True");
        }
        else {

            $(this).closest("tr").removeClass("marked");
            updateStat(Id, "False");
        }
    });

    // checking checkbox of each row and strike through them if it is checked
    function chkTab() {
        
        $('table [type="checkbox"]').each(function (i, chk) {
            if (chk.checked) {
                $(chk).closest("tr").addClass("marked");
            }
            else {
                $(chk).closest("tr").removeClass("marked");
            }
        });

    }

    // ajax call for updating Status column of taskTB
    function updateStat(Id, stat) {

        var obj = {};
        obj.taskId = parseInt(Id);
        obj.taskStat = String(stat);
      
        $.ajax({
            type: "POST",
            url: "/Home/UpdStat",
            data: JSON.stringify(obj),
            contentType: "application/json, charset=utf-8",
            dataType: 'json',
            success: function (msg) {
                if (Boolean(msg) == true) {
                    
                }
                else if (Boolean(msg) == false) {
                    
                }
            },
            error: function (msg) {
                
                console.log(msg);
            }
        });
    }

});  