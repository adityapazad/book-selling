﻿var dataTable;
$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Company/GetAll"
        },
        "columns": [
            { "data": "name", "width": "15%" },
            { "data": "streetAddress", "width": "15%" },
            { "data": "state", "width": "15%" },
            { "data": "city", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            {
                "data": "isAuthorizedCompany",
                "render": function (data) {
                    if (data) {
                        return `
<input type="checkbox" disabled checked />`
                    }
                    else {
                        return `
<input type="checkbox" disabled />`
                    }

                }
            },
            {
                "data": "id",
                "render": function (data) {
                    return `
<div class="text-center">
    <a href="/Admin/Company/Upsert/${data}" class="btn btn-info">
        <i class="fas fa-edit" "></i>
    </a>

    <a onclick=Delete("/Admin/Company/Delete/${data}") class="btn btn-danger">
        <i class="fas fa-trash-alt"></i>
    </a>
</div>
                        `;
                }
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "Delete Record?",
        text: "info",
        icon: "warning",
        buttons: true,
        dangeModel: true
    }).then((willDelte) => {
        if (willDelte) {
            $.ajax({
                url: url,
                type: "DELETE",

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
    })
}