"use strict";

function initializeSignalR() {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/quizHub") // URL của SignalR hub
        .configureLogging(signalR.LogLevel.Information)
        .build();

    connection.on("ReceiveUserJoined", function (roomCode) {
        updateRoomUser(roomCode);
    });

    function updateRoomUser(roomCode) {
        // Get the current URL parameters
        const urlParams = new URLSearchParams(window.location.search);
        const username = urlParams.get("username") || "N/A";

        // Fetch updated room users
        fetch(`/QuizRealTime/RoomUsers?roomCode=${roomCode}&username=${encodeURIComponent(username)}`)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Failed to fetch updated user list");
                }
                return response.text();
            })
            .then(html => {
                const parser = new DOMParser();
                const doc = parser.parseFromString(html, "text/html");
                const newUserList = doc.querySelector("#userList");
                if (newUserList) {
                    document.querySelector("#userList").outerHTML = newUserList.outerHTML;
                }
            })
            .catch(err => console.error("Error updating user list: ", err));
    }

    // Start the connection
    connection.start()
        .then(() => console.log("SignalR connection established"))
        .catch(err => console.error("SignalR connection error: ", err));

    // Handle connection closure and attempt reconnection
    connection.onclose(() => {
        console.log("SignalR connection closed. Attempting to reconnect...");
        setTimeout(() => startConnection(connection), 2000);
    });

    function startConnection(conn) {
        conn.start()
            .then(() => console.log("SignalR reconnected"))
            .catch(err => console.error("Reconnection failed: ", err));
    }

    return connection;
}

// Initialize SignalR on page load
document.addEventListener("DOMContentLoaded", function () {
    if (document.querySelector("#userList")) {
        initializeSignalR();
    }
});
