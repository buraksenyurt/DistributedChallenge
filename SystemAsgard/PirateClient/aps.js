const signalR = require("@microsoft/signalr");

async function sendNotification() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("http://localhost:5093/notifyHub?employeeId=PRG-1903")
        .build();

    try {
        await connection.start();
        console.log("SignalR connection established.");
        
        const notification = {
            DocumentId: "0000-0-11111111-6666-6666-6666-11111111111",
            Content: "Hello there? I am Jack Sparrow !!! :D",
            IsSuccess: true,
            Topic: "Ready"
        };

        await connection.invoke("NotifyEmployee", "PRG-1903", JSON.stringify(notification));
        console.log("Notification sent.");
    } catch (err) {
        console.error("Error establishing SignalR connection:", err);
    } finally {
        await connection.stop();
    }
}

sendNotification();
