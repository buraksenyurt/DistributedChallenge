document.addEventListener('DOMContentLoaded', (_event) => {
    const employeeId = sessionStorage.getItem('EmployeeId');
    if (!employeeId) {
        return;
    }

    const token = sessionStorage.getItem('JWToken');

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`/notifyHub?employeeId=${employeeId}`, {
            accessTokenFactory: () => token
        })
        .build();
        
    connection.on("ReadNotification", function (data) {
        const notification = JSON.parse(data);
        const popupElement = document.getElementById("notificationPopup");
        const lnkReportsElement = document.getElementById("lnkReports");

        document.getElementById("notifyMessageTopic").innerText = notification.Topic;
        document.getElementById("notifyMessageLine1").innerText = notification.Content;
        document.getElementById("notifyMessageLine2").innerText = notification.DocumentId;

        if (notification.IsSuccess) {
            popupElement.classList.remove("bg-warning", "text-dark");
            popupElement.classList.add("bg-success", "text-white");
        } else {
            popupElement.classList.remove("bg-success", "text-white");
            popupElement.classList.add("bg-warning", "text-dark");
        }

        if (notification.Topic === 'Ready') {
            lnkReportsElement.style.display = "inline";
        } else {
            lnkReportsElement.style.display = "none";
        }

        showPopup();
    });

    connection.start().then(() => {
        console.log("SignalR connection established.");
    }).catch(function (err) {
        return console.error(err.toString());
    });

    function showPopup() {
        const popup = new bootstrap.Toast(document.getElementById('notificationPopup'));
        popup.show();
        $('#notificationPopup').on('hidden.bs.toast', function () {
            refreshReports();
        });
    }

    function refreshReports() {
        $.ajax({
            url: '/Reports/Index',
            type: 'GET',
            success: function (data) {
                $('#divReports').html($(data).find('#divReports').html());
            },
            error: function (error) {
                console.error('Failed to refresh reports:', error);
            }
        });
    }

    (function () {
        'use strict';
        let forms = document.querySelectorAll('.needs-validation');
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault();
                        event.stopPropagation();
                    }
                    form.classList.add('was-validated');
                }, false);
            });
    })();
});
