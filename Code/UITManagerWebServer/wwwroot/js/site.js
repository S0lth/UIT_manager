// tps://l/client-side/bundling-and-minification
// s project to bundle and minify static web assets.


document.addEventListener("DOMContentLoaded", function () {
    const searchBox = document.getElementById("searchBoxInventory");

    if (searchBox) {
        searchBox.addEventListener("keyup", function () {
            searchInInventory();
        });
    }
});
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


document.addEventListener("click", function (event) {
    if (event.target.classList.contains("confirm-delete")) {
        const confirmed = confirm("Are you sure you want to delete this Alarm?");
        if (confirmed) {
            console.log("Alarm deleted!");
        } else {
            console.log("Deletion canceled.");
        }
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const checkboxes = document.querySelectorAll(".auto-submit-checkbox");

    checkboxes.forEach(function (checkbox) {
        checkbox.addEventListener("change", function () {
            this.form.submit();
        });
    });
});

document.addEventListener("DOMContentLoaded", function () {
    const thumbnailLinks = document.querySelectorAll(".thumbnail-link");

    thumbnailLinks.forEach(function (link) {
        link.addEventListener("click", function (event) {
            event.preventDefault();
            
            const fileId = this.getAttribute("data-file-id");
            
            const modalImage = document.getElementById("modalImage");
            if (modalImage) {
                modalImage.src = `/files/${fileId}`;
            }
        });
    });
});


document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("click", function (event) {
        if (event.target.classList.contains("update-alarm")) {
            event.preventDefault();


            const itemId = event.target.getAttribute("data-item-id");
            const statusName = event.target.getAttribute("data-status-name");


            updateAlarmStatus(itemId, statusName);
        }
    });
});

async function updateAlarmStatus(alarmId, status) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    await fetch(`/Alarm/UpdateStatus`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'X-CSRF-TOKEN': token
        },
        body: JSON.stringify({id: alarmId, status: status})
    }).then(async response => {
        if (response.ok) {
            const alertMessage = await response.text();
            document.getElementById('alert-container').innerHTML = alertMessage;
            setTimeout(() => document.getElementById('alert-container').innerHTML = '', 5000);

            const dropdownButton = document.querySelector(`#dropdownMenuButton[data-alarm-id="${alarmId}"]`);
            if (dropdownButton) {
                dropdownButton.textContent = status;
            }

            const dropdownItems = dropdownButton?.nextElementSibling.querySelectorAll('.dropdown-item');
            dropdownItems.forEach(item => {
                if (item.textContent.trim() === status) {
                    item.classList.add('active');
                } else {
                    item.classList.remove('active');
                }
            });
        }
    }).catch(error => {
    });
}

document.addEventListener("DOMContentLoaded", function () {
    const selectElement = document.querySelector("select"); // Remplacez par l'ID ou la classe du select si nécessaire

    if (selectElement) {
        selectElement.addEventListener("change", function (event) {
            const selectedOption = event.target.options[event.target.selectedIndex];

            // Vérifiez si l'option sélectionnée a les attributs nécessaires
            if (selectedOption.classList.contains("update-alarm-attribution")) {
                const itemId = selectedOption.getAttribute("data-item-id");
                const userId = selectedOption.getAttribute("data-user-id");

                // Appelez la fonction avec les données récupérées
                updateAlarmAttribution(itemId, userId);
            }
        });
    }
});

async function updateAlarmAttribution(alarmId, userId) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    try {
        const response = await fetch('/Alarm/Attribution', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': token
            },
            body: JSON.stringify({
                id: alarmId,
                userId: userId
            })
        });
        if (response.ok) {
            const alertMessage = await response.text();
            document.getElementById('alert-container').innerHTML = alertMessage;
            setTimeout(() => document.getElementById('alert-container').innerHTML = '', 5000);
        }

    } catch (error) {
        console.error('Network or server error:', error);
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

function showAlert(message, type) {
    const alertContainer = document.getElementById('alert-container');

    const alertDiv = document.createElement('div');
    alertDiv.className = `alert alert-${type} alert-dismissible fade show`;
    alertDiv.role = 'alert';
    alertDiv.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    alertContainer.appendChild(alertDiv);

    setTimeout(() => alertDiv.remove(), 5000);
}

document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("change", function (event) {
        const selectElement = event.target;
        if (selectElement.tagName === "SELECT" && selectElement.hasAttribute("data-item-id")) {
            const itemId = selectElement.getAttribute("data-item-id");
            const selectedValue = selectElement.value;
            
            updateAlarmAttribution(itemId, selectedValue);
        }
    });
});

document.getElementById('Searchh').addEventListener('keyup', function () {
    const searchValue = this.value.toLowerCase();
    const table = document.getElementById('notes-table');
    const tableRows = table.querySelectorAll('tr');
    if (tableRows.length > 1) {
        tableRows.forEach(row => {
            const cells = row.querySelectorAll('td');
            const match = Array.from(cells).some(cell => cell.textContent.toLowerCase().includes(searchValue));
            row.style.display = match ? '' : 'none';
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    document.addEventListener("click", function (event) {
        const target = event.target;

        
        if (target.classList.contains("redirect-button")) {
            event.preventDefault(); 

            
            const url = target.getAttribute("data-url");
            
            if (url) {
                window.location.href = url;
            }
        }
    });
});