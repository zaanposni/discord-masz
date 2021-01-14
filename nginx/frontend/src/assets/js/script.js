function initModCaseTable() {
    setTimeout(function () {
        $('#tableCases').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
    }, 500);
}

function initPunishmentTable() {
    setTimeout(function () {
        $('#tablePunishments').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
    }, 500);
}

function initDatatables() {
    initModCaseTable();
    initPunishmentTable();
}

function collapse(item) {
    $(item).toggleClass('moderation-big');
    $($(item).attr('data-target')).collapse('toggle');
}
