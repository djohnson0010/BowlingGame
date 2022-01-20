var games;
var currentGame;
$(function () {
    getGameList();
});
function showNewBowlingGame() {
    console.log('showing new bowling game')
    $('#mdlNewBowlingGame').modal('show');
}
function getGameList() {
    ajaxGet('/Home/getGameList/', function (result) {
        games = result;
        let tblGameList = $('#tblGameList tbody');
        tblGameList.empty();
        $.each(games, function (i, d) {
            tblGameList.append('<tr><td>' + d.id + '</td><td>' + d.name + '</td><td>' + (d.score ?? '') + '</td><td><button class="btn btn-primary" onclick="getGameInformation('+i+')">Continue</button><button class="btn btn-danger" onclick="deleteGame('+i+')">Delete</button></td>');
        })
    })
}
function getGameInformation(i) {
    let gameID = i != null ? games[i].id : currentGame.gameID 
    ajaxPost('/Home/getBowlingGame', { gameID: gameID }, function (result) {
        currentGame = result;
        console.log(result);
    });

}
function bowl(score) {
    ajaxPost('/Home/addScore', { gameID: currentGame.gameID, score }, function (result) {
        getGameInformation();
    })
}
function createBowlingGame() {
    let name = $('#txtNewGameName').val();
    ajaxPost('/Home/CreateBowlingGame', { name }, function (result) {
        if (result.code == 1) {
            $('#mdlNewBowlingGame').modal('hide');
            getGameList();
            swal.fire('Your game has been created', '', 'success')
        }          

        else {
            swal.fire(result.message, '', 'info');
        }
    })
}
function populateGameInformation() {
    $('tblGameInformation tbody').append('<tr><td>')
}
function deleteGame(i) {
    let id = games[i].id;
    Swal.fire({
        title: 'Are you sure you want to delete bowling game #'+id+'?',
        text: "You won't be able to revert this!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#3085d6',
        cancelButtonColor: '#d33',
        confirmButtonText: 'Yes'
    }).then((result) => {
        if (result.isConfirmed) {
            let id = games[i].id
            ajaxPost('/Home/deleteGame', { id }, function (result) {
                if (result.code == 1) {
                    Swal.fire(
                        'Deleted!',
                        'Your file has been deleted.',
                        'success'
                    )
                    getGameList();
                }
                   
                else {
                    swal.fire(resul.message, '', 'info')
                }
            });
           
            
        }
    })
   
}