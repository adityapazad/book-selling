﻿var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $("#tblData").DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "company.name", "width": "15%" },
            { "data": "role", "width": "15%" },
            {
                "data": {
                    id: "id", lockoutEnd: "lockoutEnd"
                },
                "render": function (data) {
                    var today = new Date().getTime();
                    var lockOut = new Date(data.lockoutEnd).getTime();
                    if (lockOut > today) {
                        //USER LOCKED
                        return `
                        <div class = "text-center">
                        <a class = "btn btn-danger" onclick=LockUnLock('${data.id}')>
                        UnLock
                        </a>
                        </div>
                        `;
                    }
                    else {
                        // user unlock
                        return `
                        <div class = "text-center">
                        <a class = "btn btn-success" onclick=LockUnLock('${data.id}')>
                        Lock
                        </a>
                        </div>
                        `;
                    }
                }
            }
        ]
    })
}

function LockUnLock(id) {
    $.ajax({
        url: "/Admin/User/LockUnlock",
        type: "POST",
        data: JSON.stringify(id),
        contentType: "application/json",
        success: function (data) {
            if (data.success) {
                toastr.success(data.message);
                dataTable.ajax.reload();
            }
            else {
                toastr.error(data.message);
            }
        }
    })
}