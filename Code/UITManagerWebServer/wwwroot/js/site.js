// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

/**
 * This function handles searching in the inventory table by filtering rows based on user input.
 * It checks all columns in each row for a match with the search input and shows/hides rows accordingly.
 */
function searchInInventory() {
    const input = document.getElementById("searchBoxInventory");
    const filter = input.value.toLowerCase();
    const table = document.getElementById("inventoryTable");
    const trs = table.getElementsByTagName("tr");

    for (let i = 1; i < trs.length; i++) {
        const cells = trs[i].getElementsByTagName("td");
        let rowContainsSearch = false;

        for (let cell of cells) {
            const txtValue = cell.textContent || cell.innerText;
            if (txtValue.toLowerCase().indexOf(filter) > -1) {
                rowContainsSearch = true;
                break; 
            }
        }

        trs[i].style.display = rowContainsSearch ? "" : "none";
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