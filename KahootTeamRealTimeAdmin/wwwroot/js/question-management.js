"use strict";

// Create connection
var connection = new signalR.HubConnectionBuilder()
    .withUrl("/quizHub")
    .build();

// Handle receiving question deleted notification
connection.on("QuestionDeleted", function (questionId) {
    // Remove the question row from the table
    var row = document.querySelector(`tr[data-question-id='${questionId}']`);
    if (row) {
        row.remove();
    }

    // Show notification
    showNotification("Question has been deleted.");
});

// Handle receiving question removed from room notification
connection.on("QuestionRemovedFromRoom", function (roomId, questionId) {
    // Only update if we're on the correct room page
    var currentRoomId = document.getElementById("current-room-id").value;
    if (currentRoomId === roomId) {
        var row = document.querySelector(`tr[data-question-id='${questionId}']`);
        if (row) {
            row.remove();
        }

        // Show notification
        showNotification("Question has been removed from this room.");
    }
});

connection.on("AnswerDeleted", function (answerId, questionId) {
    // Remove the answer row from the table
    var row = document.querySelector(`tr[data-answer-id='${answerId}']`);
    if (row) {
        row.remove();

        // Show notification
        showNotification("Answer has been deleted.");

        // Update the answers count in the questions list if present
        var questionRow = document.querySelector(`tr[data-question-id='${questionId}']`);
        if (questionRow) {
            var countCell = questionRow.querySelector("td:nth-child(2)");
            if (countCell) {
                var currentCount = parseInt(countCell.textContent);
                if (!isNaN(currentCount)) {
                    countCell.textContent = (currentCount - 1).toString();
                }
            }
        }
    }
});

// Show notification
function showNotification(message) {
    var notification = document.createElement("div");
    notification.className = "alert alert-info alert-dismissible fade show";
    notification.innerHTML = `
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
    `;

    var container = document.querySelector(".container");
    container.insertBefore(notification, container.firstChild);

    // Auto dismiss after 5 seconds
    setTimeout(function () {
        notification.remove();
    }, 5000);
}

// Start connection
connection.start().catch(function (err) {
    return console.error(err.toString());
});