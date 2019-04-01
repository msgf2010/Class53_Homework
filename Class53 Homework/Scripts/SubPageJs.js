$(() => {
    $("#new-simcha").on('click', function () {
        $("#new-simcha-modal").modal();
    });
})

$(() => {
    $("#contributer-table").on('click', '.deposit', function () {
        const Id = $(this).attr('id');
        $(".modal-title").text(`Add deposit for id ${Id}`);

        $("#d-amount").val('');
        $("#d-date").val('');
        $("#d-id").val(Id);

        $("#deposit-modal").modal();
    });
})