document.addEventListener('DOMContentLoaded', (_event) => {
    const employeeIdElement = document.getElementById('employeeId');
    if (!employeeIdElement) {
        return;
    }
    const employeeId = employeeIdElement.value;

    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/notifyHub?employeeId=" + employeeId)
        .build();

    connection.on("ReadNotification", function (data) {
        const notification = JSON.parse(data);
        console.log(notification);
        const popupElement = document.getElementById("notificationPopup");
        const lnkReportsElement = document.getElementById("lnkReports");

        document.getElementById("notifyMessageTopic").innerText = notification.Topic;
        document.getElementById("notifyMessageLine1").innerText = notification.Content;
        document.getElementById("notifyMessageLine2").innerText = notification.DocumentId;

        if (notification.IsSuccess && notification.Topic === 'Report') {
            popupElement.classList.remove("bg-warning", "text-dark");
            popupElement.classList.add("bg-success", "text-white");
            lnkReportsElement.style.display = "inline";
        } else {
            popupElement.classList.remove("bg-success", "text-white");
            popupElement.classList.add("bg-warning", "text-dark");
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
    }

    (function () {
        'use strict'
        let forms = document.querySelectorAll('.needs-validation')
        Array.prototype.slice.call(forms)
            .forEach(function (form) {
                form.addEventListener('submit', function (event) {
                    if (!form.checkValidity()) {
                        event.preventDefault()
                        event.stopPropagation()
                    }
                    form.classList.add('was-validated')
                }, false)
            })
    })()
});
