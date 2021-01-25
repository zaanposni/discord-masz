var caseTable;
var punishmentTable;

function initModCaseTable() {
    setTimeout(function () {
        caseTable = $('#tableCases').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
    }, 500);
}

function initPunishmentTable() {
    setTimeout(function () {
        punishmentTable = $('#tablePunishments').DataTable({
            pageLength: 100,
            order: [0, "desc"]
        });
    }, 500);
}

function cExcludeAutoModeration() {
    caseTable.column(2).search('[0|2-9]', true).draw();
}

function pExcludeAutoModeration() {
    punishmentTable.column(2).search('[0|2-0]', true).draw();
}

function cResetAutoModeration() {
    caseTable.column(2).search('').draw();
}

function pResetAutoModeration() {
    punishmentTable.column(2).search('').draw();    
}

// ================


function cExcludePermaPunishments() {
    caseTable.column(3).search('false').draw();
}

function pExcludePermaPunishments() {
    punishmentTable.column(3).search('false').draw();
}

function cResetPermaPunishments() {
    caseTable.column(3).search('').draw();
}

function pResetPermaPunishments() {
    punishmentTable.column(3).search('').draw();    
}

// ================

function initDatatables() {
    initModCaseTable();
    initPunishmentTable();
}

function collapse(item) {
    $(item).toggleClass('moderation-big');
    $($(item).attr('data-target')).collapse('toggle');
}
