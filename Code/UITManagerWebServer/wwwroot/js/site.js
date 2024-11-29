// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

function updateAlarmStatus(alarmId, status) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
    if (!token) {
        alert('CSRF token is missing.');
        return;
    }

    fetch(`/Alarms/UpdateStatus`, {
        method: 'POST',
        headers: {
        'Content-Type': 'application/json',
        'X-CSRF-TOKEN': token
    },
        body: JSON.stringify({id: alarmId, status: status})
    }).then(response => {
        if (!response.ok) {
        alert('Error updating status.');
    }
    }).catch(error => {
        alert('Network error updating status.');
    });
}

/**
 * Filters rows in the "inventoryTable" dynamically based on the search query entered in the search box with ID "searchBoxHostInventory".
 *
 * This function is triggered on every keystroke in the search box (via the `onkeyup` event).
 */
function searchHostInInventory() {
    var input = document.getElementById("searchBoxHostInventory");
    var filter = input.value.toLowerCase();
    var table = document.getElementById("inventoryTable");
    var trs = table.getElementsByTagName("tr");

    for (var i = 1; i < trs.length; i++) {
        var td = trs[i].getElementsByTagName("td")[0];
        if (td) {
            var txtValue = td.textContent || td.innerText;
            if (txtValue.toLowerCase().indexOf(filter) > -1) {
                trs[i].style.display = "";
            } else {
                trs[i].style.display = "none";
            }
        }
    }
}