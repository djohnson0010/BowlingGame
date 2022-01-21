var games;
var currentGame;
var remainingPins;
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
            tblGameList.append('<tr><td>' + d.id + '</td><td>' + d.name + '</td><td>' + (d.score ?? '') + '</td><td><button class="btn btn-primary" onclick="getGameInformation('+d.id+')">Continue</button><button class="btn btn-danger" onclick="deleteGame('+i+')">Delete</button></td>');
        })
    })
}
function getGameInformation(id) {
    let gameID = id
    ajaxPost('/Home/getBowlingGame', { gameID: gameID }, function (result) {
        console.log(result);
        currentGame = result;
        $('#tblGameInformation thead').empty();
        $('#tblGameInformation tbody').empty();
        let rowFrames = '<tr><th>Frames</th>'
        let rowScores = '<tr><td>Scores</td>'
        let rowPoints = '<tr><td>Final</td>'
        for (frame = 0; frame < 10; frame++)
        {
            let currentFrame = currentGame.Frames[frame];
            let colspan = frame == 9 ? 3 : 2;
            rowFrames += '<th colspan="' + colspan + '">' + (frame + 1) + '</th>';
                let frameScore1 = currentFrame != null && currentFrame.scores[0] != null ? convertScore(currentFrame.scores[0])  : '';
            let frameScore2 = currentFrame != null &&  currentFrame.scores[1] != null ? convertScore(currentFrame.scores[1]) : '';
            let frameScore3 = currentFrame != null && currentFrame.scores[2] != null ? convertScore(currentFrame.scores[2]) : '';
            if (frame == 9) {
                rowScores += '<td>' + frameScore1 + '</td><td>' + frameScore2 +'</td><td>'+frameScore3+'</td>'
            }
            else {
                rowScores += '<td>' + frameScore1 + '</td><td>' + frameScore2 +'</td>'
            }
            
            rowPoints += '<td colspan="'+colspan+'">' + (currentFrame != null ? currentFrame.frameScore : '') + '</td>'
        }
        $('#tblGameInformation thead').append(rowFrames)
        $('#tblGameInformation tbody').append(rowScores)
        $('#tblGameInformation tbody').append(rowPoints)
        
    });

}
function convertScore(score) {
    return (score.isStrike ? 'X': score.isSpare ? '/' : score.scoreNumber)
}
function bowlBall() {
    bowl(Math.floor(Math.random() * (currentGame.bowlingPins+1)))
}
function bowl(score) {
    ajaxPost('/Home/addScore', { gameID: currentGame.gameID, score }, function (result) {
        getGameInformation(currentGame.gameID);
    })
}
function createBowlingGame() {
    let name = $('#txtNewGameName').val();
    ajaxPost('/Home/CreateBowlingGame', { name }, function (result) {
        if (result.code == 1) {
            $('#mdlNewBowlingGame').modal('hide');
            getGameList();
            swal.fire('Your game has been created', '', 'success')
            getGameInformation(result.gameID)
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