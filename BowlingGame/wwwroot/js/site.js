// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function ajaxPost(url, data, callback) {
    return $.ajax({
        url: url,
        type: "POST",
        data: data,
        success: function (result) {
            callback(result)
        },
        error: function (error) {
            console.log(error.responseText)
            swal.fire({
                title: 'An Error Occured',
                icon: 'error',
                showConfirmButton: true
            })
        },
        complete: function () { }
    });
}
function ajaxGet(url, callback) {
    return $.ajax({
        url: url,
        type: "Get",
        success: function (result) {
            callback(result)
        },
        error: function (error) {
            console.log(error.responseText)
            swal.fire({
                title: 'An Error Occured',
                icon: 'error',
                showConfirmButton: true
            })
        },
        complete: function () { }
    });
}