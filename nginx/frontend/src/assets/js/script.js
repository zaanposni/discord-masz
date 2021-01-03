function initDatatables() {
    setTimeout(function () {
        $('#tableCases').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
        $('#tablePunishments').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
    }, 1000);
}

function collapse(item) {
    $(item).toggleClass('moderation-big');
    $($(item).attr('data-target')).collapse('toggle');
}
