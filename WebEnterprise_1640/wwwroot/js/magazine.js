// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// site.js
var originalMagazineOrder = [];

$(document).ready(function () {
    $('.magazine-card').each(function () {
        originalMagazineOrder.push($(this));
    });

    $('.col-md-6').each(function () {
        originalMagazineOrder.push($(this).find('.magazine-card'));
    });
});

function searchMagazines() {
    var searchTerm = $('#magazineSearch').val().toLowerCase();

    originalMagazineOrder.forEach(function (magazine) {
        var magazineName = magazine.find('.card-title').text().toLowerCase();
        if (magazineName.includes(searchTerm)) {
            magazine.parent().show();
        } else {
            magazine.parent().hide();
        }
    });
}
$(document).ready(function () {
    $('.magazine-link').click(function (e) {
        e.preventDefault();
        var magazineId = $(this).data('magazine-id');
        $.ajax({
            url: '/Magazines/GetArticles/' + magazineId,
            type: 'GET',
            success: function (data) {
                // Xử lý dữ liệu trả về và hiển thị bài báo tương ứng
            },
            error: function () {
                alert('Đã xảy ra lỗi khi lấy danh sách bài báo.');
            }
        });
    });
});
