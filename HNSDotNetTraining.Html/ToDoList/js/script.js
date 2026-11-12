
// =====================================
// TODO LIST APPLICATION
// =====================================

const TblName = "TodoList";

let editId = null;

let currentFilter = "all";


// =====================================
// INITIAL LOAD
// =====================================

$(document).ready(function () {

    loadData();

});


// =====================================
// SAVE / UPDATE BUTTON
// =====================================

$("#taskForm").submit(function (e) {

    e.preventDefault();

    let title = $("#TaskTitle").val().trim();

    if (title === "") {

        alert("Please enter task title.");

        $("#TaskTitle").focus();

        return;

    }

    if (editId === null) {

        SaveData();

    } else {

        updateData();

    }

});


// =====================================
// CANCEL EDIT
// =====================================

$("#btnCancel").click(function () {

    resetForm();

});


// =====================================
// SEARCH TASK
// =====================================

$("#searchTask").on("input", function () {

    loadData();

});


// =====================================
// FILTER TASK
// =====================================

$(".filter-btn").click(function () {

    $(".filter-btn").removeClass("active");

    $(this).addClass("active");

    currentFilter = $(this).data("filter");

    loadData();

});


// =====================================
// GET TASK DATA
// =====================================

function getTasks() {

    let existingData = localStorage.getItem(TblName);

    if (existingData === null) {

        return [];

    }

    try {

        let tasks = JSON.parse(existingData);

        return Array.isArray(tasks) ? tasks : [];

    } catch (error) {

        console.error("Invalid task data:", error);

        return [];

    }

}


// =====================================
// SAVE NEW TASK
// =====================================

function SaveData() {

    let title = $("#TaskTitle").val().trim();

    let description = $("#TaskDescription").val().trim();

    let item = {

        Id: crypto.randomUUID(),

        title: title,

        description: description,

        isCompleted: false

    };

    let lst = getTasks();

    lst.push(item);

    localStorage.setItem(TblName, JSON.stringify(lst));

    resetForm();

    loadData();

}


// =====================================
// UPDATE TASK
// =====================================

function updateData() {

    let title = $("#TaskTitle").val().trim();

    let description = $("#TaskDescription").val().trim();

    let lst = getTasks();

    let index = lst.findIndex(x => x.Id === editId);

    if (index === -1) {

        alert("Task not found.");

        resetForm();

        return;

    }

    lst[index].title = title;

    lst[index].description = description;

    localStorage.setItem(TblName, JSON.stringify(lst));

    resetForm();

    loadData();

}


// =====================================
// EDIT TASK
// =====================================

function editItem(Id) {

    let lst = getTasks();

    let item = lst.find(x => x.Id === Id);

    if (!item) {

        alert("Task not found.");

        return;

    }

    editId = item.Id;

    $("#TaskTitle").val(item.title);

    $("#TaskDescription").val(item.description);

    $("#formHeading").text("Edit Task");

    $("#btnSave").text("Update Task");

    $("#btnCancel").removeClass("hidden");

    // Scroll to form
    $("html, body").animate({

        scrollTop: $("#taskForm").offset().top - 100

    }, 300);

}


// =====================================
// DELETE TASK
// =====================================

function deleteItem(Id) {

    if (!confirm("Are you sure you want to delete this task?")) {

        return;

    }

    let lst = getTasks();

    let updatedList = lst.filter(x => x.Id !== Id);

    localStorage.setItem(TblName, JSON.stringify(updatedList));

    if (editId === Id) {

        resetForm();

    }

    loadData();

}


// =====================================
// COMPLETE / UNDO TASK
// =====================================

function toggleComplete(Id) {

    let lst = getTasks();

    let item = lst.find(x => x.Id === Id);

    if (!item) {

        return;

    }

    item.isCompleted = !item.isCompleted;

    localStorage.setItem(TblName, JSON.stringify(lst));

    loadData();

}


// =====================================
// LOAD TASK DATA
// =====================================

function loadData() {

    let lst = getTasks();


    // Dashboard Statistics

    let total = lst.length;

    let completed = lst.filter(
        x => x.isCompleted
    ).length;

    let pending = total - completed;


    $("#totalTasks").text(total);

    $("#pendingTasks").text(pending);

    $("#completedTasks").text(completed);


    // Search

    let searchText = $("#searchTask")
        .val()
        .trim()
        .toLowerCase();


    // Search + Filter

    let filteredList = lst.filter(item => {

        let matchesSearch = item.title
            .toLowerCase()
            .includes(searchText);

        let matchesFilter = true;

        if (currentFilter === "pending") {

            matchesFilter = !item.isCompleted;

        }

        if (currentFilter === "completed") {

            matchesFilter = item.isCompleted;

        }

        return matchesSearch && matchesFilter;

    });


    bindData(filteredList);

}


// =====================================
// ESCAPE HTML
// =====================================

function escapeHTML(value) {

    return String(value ?? "").replace(/[&<>"']/g, function (char) {

        const entities = {

            "&": "&amp;",

            "<": "&lt;",

            ">": "&gt;",

            '"': "&quot;",

            "'": "&#39;"

        };

        return entities[char];

    });

}


// =====================================
// DISPLAY TASK CARDS
// =====================================

function bindData(lst) {

    let cards = "";

    $("#taskCount").text(lst.length + " Tasks");


    if (lst.length === 0) {

        $("#taskList").html("");

        $("#emptyMessage").removeClass("hidden");

        return;

    }

    $("#emptyMessage").addClass("hidden");


    for (let i = 0; i < lst.length; i++) {

        let item = lst[i];

        let status = item.isCompleted
            ? "Completed"
            : "Pending";

        let statusClass = item.isCompleted
            ? "bg-emerald-100 text-emerald-700"
            : "bg-amber-100 text-amber-700";

        let cardClass = item.isCompleted
            ? "completed"
            : "";

        let completeText = item.isCompleted
            ? "Undo"
            : "Complete";

        let safeId = escapeHTML(item.Id);


        cards += `

            <div class="task-card ${cardClass}">

                <div class="flex items-start justify-between gap-3 mb-3">

                    <div class="min-w-0 flex-1">

                        <h3 class="task-title font-semibold text-slate-800
                                   break-words">

                            ${escapeHTML(item.title)}

                        </h3>

                    </div>

                    <span class="${statusClass} text-xs font-semibold
                                 px-3 py-1 rounded-full whitespace-nowrap">

                        ${status}

                    </span>

                </div>


                <p class="text-sm text-slate-500 mb-5 whitespace-pre-wrap
                          break-words">

                    ${escapeHTML(item.description || "No description")}

                </p>


                <div class="flex flex-wrap gap-2 pt-3 border-t border-slate-100">

                    <button

                        type="button"

                        class="action-btn complete-btn"

                        onclick="toggleComplete('${safeId}')">

                        ${completeText}

                    </button>


                    <button

                        type="button"

                        class="action-btn edit-btn"

                        onclick="editItem('${safeId}')">

                        Edit

                    </button>


                    <button

                        type="button"

                        class="action-btn delete-btn"

                        onclick="deleteItem('${safeId}')">

                        Delete

                    </button>

                </div>

            </div>

        `;

    }


    $("#taskList").html(cards);

}


// =====================================
// RESET FORM
// =====================================

function resetForm() {

    editId = null;

    $("#TaskTitle").val("");

    $("#TaskDescription").val("");

    $("#formHeading").text("Create a Task");

    $("#btnSave").text("+ Add Task");

    $("#btnCancel").addClass("hidden");

}
