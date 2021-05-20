tableLangSettings = {
    search: "Pretraga",
    info: "_START_ - _END_ od ukupno _TOTAL_ zapisa",
    lengthMenu: "Prikaži _MENU_ zapisa",
    paginate: {
        first: "Prva",
        previous: "Prethodna",
        next: "Sljedeća",
        last: "Zadnja"
    },
    emptyTable: "Nema podataka za prikaz",
    infoEmpty: "Nema podataka za prikaz",
    infoFiltered: "(filtrirano od ukupno _MAX_ zapisa)",
    zeroRecords: "Ne postoje traženi podaci"
};

function clearOldMessage() {
  $("#tempmessage").siblings().remove();
  $("#tempmessage").removeClass("alert-success");
  $("#tempmessage").removeClass("alert-danger");
  $("#tempmessage").html('');
}

//Pri svakom kliku kontrole koja ima css klasu delete zatraži potvrdu
//za razliku od  //$(".delete").click ovo se odnosi i na elemente koji će se pojaviti u budućnosti 
//dinamičkim učitavanjem
$(function () {
  $(document).on('click', '.delete', function (event) {
    if (!confirm("Obrisati zapis?")) {
      event.preventDefault();
    }
    else {
      const url = $(this).data('delete-ajax'); //Provjeri radi li se dinamičko brisanje (ako je definirana adresa u data-delete-ajax)
      if (url !== undefined && url !== '') {
        event.preventDefault();
        deleteAjax($(this), url);        
      }
    }
  });

  $(document).on('click', '.edit', function (event) {
    const editUrl = $(this).data('edit-ajax');
    if (editUrl !== undefined && editUrl !== '') {
      const getUrl = $(this).data('get-ajax');
      event.preventDefault();
      editAjax($(this), editUrl, getUrl);
    }
  });
});

function deleteAjax(btn, url) {  
  const tr = btn.parents("tr");//redak kojem zapis pripada
  const active = tr.data('active');//flag da je akcija u tijeku (da spriječimo 2 brza klika)
  if (active !== true) {
    tr.data('active', true);
    clearOldMessage();
        
    const token = $('input[name="__RequestVerificationToken"]').first().val();    
    
    $.post(url, { __RequestVerificationToken: token }, function (data) {
      if (data.successful) { //metoda mora vratiti JSON koji sadrži message i successful
        tr.remove();
      }
      $("#tempmessage").addClass(data.successful ? "alert-success" : "alert-danger");
      $("#tempmessage").html(data.message);
    }).fail(function (jqXHR) {
      alert(jqXHR.status + " : " + jqXHR.responseText);
      tr.data('active', false);
    });
  }
}

function editAjax(btn, editUrl, getUrl) {
  const tr = btn.parents("tr");//redak kojem zapis pripada
  const active = tr.data('active');//flag da je akcija u tijeku (da spriječimo 2 brza klika)
  if (active !== true) {
    tr.data('active', true);
    clearOldMessage();
    
    $.get(editUrl, {  }, function (data) {
      tr.toggle(); //sakrij trenutni redak
      var inserted = $(data).insertAfter(tr); //iza skrivenog retka dodaj redak koji je došao sa servera
      setCancelAndSaveBehaviour(tr, inserted, editUrl, getUrl);
    })
    .fail(function (jqXHR) {
      alert(jqXHR.status + " : " + jqXHR.responseText);
      tr.data('active', false);
    });
  }
}

function setCancelAndSaveBehaviour(hiddenRow, insertedData, editUrl, getUrl) {
  //nađi cancel button 
  insertedData.find(".cancel").click(function (event) {
    insertedData.remove(); //ukloni umetnuti redak
    hiddenRow.toggle(); //vrati vidljivost originalnom retku
    hiddenRow.data('active', false);
  });

  //nađi save button 
  insertedData.find(".save").click(function (event) {
    event.preventDefault(); //u slučaju da se radi o nekom submit buttonu, inače nije nužno    
    var formData = collectData(insertedData);

    $.ajax({
      type: "POST",
      url: editUrl,
      contentType: false,
      processData: false,
      data: formData,
      success: function (data, textStatus, jqXHR) {
        insertedData.remove();
        if (jqXHR.status == 204) {//No content (znači da je podatak ažuriran)          
          $.get(getUrl, {}, function (refreshedRow) {
            $(hiddenRow).replaceWith(refreshedRow);
          })
        }
        else { //200                    
          var inserted = $(data).insertAfter(hiddenRow);
          setCancelAndSaveBehaviour(hiddenRow, inserted, editUrl, getUrl);
        }                
      },
      error: function (jqXHR) {
        alert(jqXHR.status + " : " + jqXHR.responseText);
      }
    });
  });
}


function collectData(container) {
  //pripremi podatke i spremi ih u json
  var formData = new FormData();

  //pronađi sve elemente koji imaju data-save (mogli bi tražiti i data-val='true', ali što kao ASP.Net promijeni način označavanja
  container.find("[data-save]").each(function (index, element) {
    //dodaj vrijednost elementa u object data koji će kasnije biti poslan na server 
    if ($(element).is(':checkbox')) {
      formData.append($(element).attr('name'), $(element).is(':checked'));
    }
    else if ($(element).is("input[type=file]")) {
      var files = $(element).get(0).files;
      if (files.length > 0) {
        formData.append($(element).attr('name'), files[0]);
      }
    }
    else {
      var val = $.trim($(element).val());
      if (val != '') {
        formData.append($(element).attr('name'), val);
      }
    }
  });


  //find antiforgery token   
  var token = $('input[name="__RequestVerificationToken"]').first().val();
  formData.append("__RequestVerificationToken", token);

  return formData;
}
