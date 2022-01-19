var games;
$(function () {
    getGameList();
});
function showNewBowlingGame() {
    console.log('showing new bowling game')
    $('#mdlNewBowlingGame').modal('show');
}
function getGameList() {
    ajaxPost('/Home/getGameList/', {}, function (result) {
        let tblGameList = $('#tblGameList tbody');
        tblGameList.empty();
        $.each(games, function (i, d) {
            tblGameList.append('<tr><td>' + d.gameID + '</td><td>' + d.name + '</td><td>' + d.score + '</td><td>');
        })
    })
}
function createBowlingGame() {
    let name = $('#txtNewGameName').val();
    ajaxPost('/Home/CreateBowlingGame', { name }, function (result) {
        if (result.code == 1)
            getGame();

        else {
            swal.fire(result.message);
        }
    })
}