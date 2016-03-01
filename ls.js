$(function () {

    // Declare a proxy to reference the hub.
    var blas = $.connection.bLAS;
    var i = 0;
    var data = [];
    var displayOutput = "[";
    // Start the connection.
    $.connection.hub.start().done(function () {
        alert('Now connected, connection ID=' + $.connection.hub.id)
        $('#send').click(function () {
            var mat1JSON = $('#mat1').val();
            var mat2JSON = $('#mat2').val();
            // Call the LU decomposition method on the hub.
            blas.server.blas1(mat1JSON, mat2JSON, $.connection.hub.id);
        });
    });
    // Create a function that the hub can call to display the error message.
    blas.client.displayError1 = function () {        
        document.getElementById("Product").innerHTML = 'Input Matrix A should be a square matrix';   
    };
    // Create a function that the hub can call to store the product of a matrix and column vector.
    blas.client.store = function (product) {        
        var productObj = JSON.parse(product);
        data[i] = productObj;
        i++;
    };
    //Create a function that hub can call to display the final output
    blas.client.displayOutput = function () {
        for (i = 0; i < data.length; i++) {
            displayOutput += '[' + data[i] + ']';
            if (i != data.length - 1) {
                displayOutput += ','
            } else displayOutput += ']'
        }
        document.getElementById("Product").innerHTML = 'The output is ' + displayOutput;
        displayOutput = "[";
        i = 0;
        data = [];
    };

    blas.client.displayError2 = function (m, k) {
        var productObj = JSON.parse(m);
        var productObj1 = JSON.parse(k);
        document.getElementById("Product").innerHTML = 'length of B Matrix' + productObj + ' is not equal to A matrix' + productObj1;
    };
});
