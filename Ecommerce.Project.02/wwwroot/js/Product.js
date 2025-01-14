var dataTable;

$(document).ready(function () {
    loadDataTable();
});
function loadDataTable() {
    dataTable = $('#tblData').DataTable({
        "ajax": { "url": "/Admin/Product/GetAll" },
        "columns": [
            {"data" : "title", "width" : "15%"},
            {"data" : "description", "width" : "20%"},
            {"data" : "author", "width" : "15%"},
            {"data" : "isbn", "width" : "10%"},
            { "data": "price", "width": "10%" },
            {
                "data": "id", "width": "30%",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a class = "btn btn-info" href="/Admin/Product/Upsert/${data}">
                                <i class="fas fa-edit"></i>
                            </a>
                        
                            <a class = "btn btn-danger" onclick="Delete('/Admin/Product/Delete/${data}')">
                                <i class="fas fa-trash"></i>
                            </a>
                        </div>
                    `
                } 
            }
        ]
    })
}

function Delete(url) {
    swal({
        title: "WANT TO DELETE !",
        text: "information",
        icon: "warning",
        buttons: true,
        dangeModel: true
    }).then((willDelete) => {
        if (willDelete) {
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