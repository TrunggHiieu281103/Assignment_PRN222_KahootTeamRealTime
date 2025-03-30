"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/quizHub").build();

connection.start().then(function () {
    console.log("Connected to QuizHub");

    var roomCode = document.getElementById("roomCode").textContent;
    var username = document.getElementById("username").textContent;

    connection.invoke("JoinRoom", roomCode, username).catch(function (err) {
        return console.error("Error joining room:", err.toString());
    });
}).catch(function (err) {
    console.error("Error connecting to QuizHub:", err.toString());
});

// Cập nhật danh sách người chơi
connection.on("UpdateRoomUsers", function (users) {
    const userList = document.getElementById("userList");
    userList.innerHTML = "";

    users.forEach(user => {
        const li = document.createElement("li");
        li.textContent = user;
        userList.appendChild(li);
    });

    console.log("Updated room users:", users);
});

// Gửi yêu cầu rời phòng khi người chơi nhấn "Leave Room"
document.getElementById("leaveRoomButton").addEventListener("click", function () {
    var roomCode = document.getElementById("roomCode").textContent;
    var username = document.getElementById("username").textContent;

    connection.invoke("LeaveRoom", roomCode, username).catch(function (err) {
        return console.error("Error leaving room:", err.toString());
    });

    window.location.href = "/QuizRealTime/JoinRoom"; // Chuyển hướng về trang Join Room
});
