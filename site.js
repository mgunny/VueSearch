
// Create a Global Variable for the Vue Event Bus - to pass info from components
const EventBus = new Vue();

// Set Snotify Defaults
Snotify.setDefaults({ global: { preventDuplicates: false }, toast: { position: "rightBottom", timeout: 2000 } });


// Define a Vue Shopping Cart Component
Vue.component('shopping-cart-button', {

    props: ['productsku', 'addtocart'],

    template: `
        <div class="cart-button">           
            <button class="btn btn-success ANLTProextrasmallwhite mb-2" v-on:click="addProduct(productsku)"><span class="fa fa-shopping-cart mr-1"></span> {{addtocart}}</button>
            <br /><img src="/images/WE_Logo.png" style="max-width: 100px;" alt="Wolselsey Express Logo">
        </div>
    `,
    
    methods: {

        addProduct(sku) {           
            EventBus.$emit('update-mini-cart', sku); // Update Mini Cart Total
        }
    }

});

