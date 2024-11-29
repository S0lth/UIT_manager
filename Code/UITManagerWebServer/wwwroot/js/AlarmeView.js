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


async function updateAlarmAttribution(alarmId, userId) {
    const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;

    if (!token) {
        alert('CSRF token is missing.');
        return;
    }

    if (!alarmId || !userId) {
        alert('Invalid alarm or user ID.');
        return;
    }

    try {
        const response = await fetch('/Alarms/Attribution', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'X-CSRF-TOKEN': token // Ajout du token pour la protection CSRF
            },
            body: JSON.stringify({
                id: alarmId, // ID de l'alarme
                userId: userId // ID de l'utilisateur
            })
        });

        if (response.ok) {
            const result = await response.json();
        } else {
            alert('Error: Failed to update attribution. Status code: ' + response.status);
        }
    } catch (error) {
        console.error('Network or server error:', error);
        alert('An error occurred while updating the attribution.');
    }
}


document.addEventListener('DOMContentLoaded', function () {
    const savedValue = localStorage.getItem('selectedFilter');
    if (savedValue) {
        const selectElement = document.getElementById('filter');
        if (selectElement) {
            selectElement.value = savedValue;
        }
    }
});

function saveSelection(value) {
    localStorage.setItem('selectedFilter', value);
}


function redirectToSortOrder(sortOrder) {
    saveSelection(sortOrder);
    const url = `/Alarms/Index?sortOrder=${encodeURIComponent(sortOrder)}`;
    window.location.href = url;
}


function handleSelectChange(selectElement) {
    const selectedValue = selectElement.value;

    const url = `/ControllerName/Index?sortOrder=${selectedValue}`;
    window.location.href = url;
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
