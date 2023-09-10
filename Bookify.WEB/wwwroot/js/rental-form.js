var selectedCopies = [];

$(document).ready(function () {
    $('.js-search').on('click', function (e) {
        e.preventDefault();

        var serial = $('#Value').val();

        if (selectedCopies.find(c => c.serial == serial)) {
            showErrorMessage('You cannot add the same copy');
            return;
        }
        
        if (selectedCopies.length >= maxAllowedCopies) {
            showErrorMessage(`You cannot add more that ${maxAllowedCopies} books`);
            return;
        }

        $('#SearchForm').submit();
    });

    $('body').delegate('.js-remove', 'click', function () {
        $(this).parents('.js-copy-container').remove();
        prepareInput();
        if ($.isEmptyObject(selectedCopies))
            $('#CopiesForm').find(':submit').addClass('d-none');
    })
});

function onAddCopySuccess(copy) {
    $('#Value').val('');
    //console.log(copy);
    var bookId = $(copy).find('.js-copy').data('book-id');

    if (selectedCopies.find(c => c.bookId == bookId)) {
        showErrorMessage('You cannot add more than one copy for the same book');
        return;
    }

    $('#CopiesForm').prepend(copy);
    $('#CopiesForm').find(':submit').removeClass('d-none');

    prepareInput();
}
function prepareInput() {
    var copies = $('.js-copy');

    selectedCopies = [];

    $.each(copies, function (i, input) {
        var $input = $(input);
        selectedCopies.push({ serial: $input.val(), bookId: $input.data('book-id') });
        $input.attr('name', `SelectedCopies[${i}]`).attr('id', `SelectedCopies_${i}_`);
    });
}