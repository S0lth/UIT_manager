// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * This function handles searching in the inventory table by filtering rows based on user input.
 * It checks all columns in each row for a match with the search input and shows/hides rows accordingly.
 */
function searchInInventory() {
    const input = document.getElementById("searchBoxInventory").value.toLowerCase();
    const table = document.getElementById("inventoryTable");
    const trs = table.getElementsByTagName("tr");
    let hasResults = false;

    for (let i = 1; i < trs.length; i++) {
        const rowText = trs[i].innerText.toLowerCase();
        const isMatch = rowText.includes(input);
        trs[i].style.display = isMatch ? "" : "none";
        if (isMatch) hasResults = true;
    }

    const noResultDivId = "noResultMessage";
    let noResultDiv = document.getElementById(noResultDivId);

    if (!hasResults) {
        if (!noResultDiv) {
            noResultDiv = document.createElement("div");
            noResultDiv.id = noResultDivId;
            noResultDiv.className = "list-group-item text-center";
            noResultDiv.innerHTML = `
                <div class="alert alert-secondary" role="alert">
                    No machine found
                </div>
            `;
            table.parentNode.appendChild(noResultDiv);
        }
    } else {
        if (noResultDiv) {
            noResultDiv.remove();
        }
    }
}


document.getElementById('Search').addEventListener('keyup', function () {
    const searchValue = this.value.toLowerCase();
    const tableRows = document.querySelectorAll('table tbody tr');

    tableRows.forEach(row => {
        const cells = row.querySelectorAll('td');
        let match = false;

        cells.forEach(cell => {
            if (cell.textContent.toLowerCase().includes(searchValue)) {
                match = true;
            }
        });


        if (match) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
});