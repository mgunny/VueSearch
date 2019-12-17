function ActivateShoppingCart(culture, resourceStrings) {

    Vue.use(VueClazyLoad);

    var app = new Vue({
        el: '#cart',
        mounted() {
            this.culture = culture;
            this.resourceStrings = JSON.parse(resourceStrings);

            // Load Shopping Cart Details
            this.getMainCategories(culture);
            this.getCartDetails(culture);

        },
        components: { VueIntlNumberformat: VueIntlNumberformat },
        data() {
            return {
                culture: "",
                loading: false,
                notFound: false,
                resourceStrings: [],
                loadingMainCatgs: false,
                advertLeft: {},
                mainCategories: [],
                products: [],
                productDetailsList: []
            };
        },

        computed: {
            formData() {

                // Get Culture code as 2 character capitalised string eg. "En" or "Fr"
                var lang = this.culture.split("-")[0];
                lang = lang.charAt(0).toUpperCase() + lang.slice(1);

                // Create model to submit to W/E Api
                var data = { Language: lang, Products: [] };
                this.products.map(function (value, key) { data.Products.push({ SKU: "SIL" + value }); });
                var jsonStringify = JSON.stringify(data);
                return jsonStringify;
            },

            cartTotal() {
                var total = 0;
                this.productDetailsList.map(function (p) { total = total + p.price; });                
                return total;
            }
        },

        watch: {
            products(newProducts) {
                localStorage.cart = newProducts.toString();
            }
        },

        methods: {
           
            getCartDetails(culture) {               
                if (localStorage.cart) {
                    this.products = localStorage.cart.split(",");
                    this.getShoppingCartProducts(culture);
                }
            },

            deleteProduct(product) {                
                var productIndex = this.products.indexOf(product.productSku);
                var productDetailIndex = this.productDetailsList.indexOf(product);
                this.$delete(this.products, productIndex);
                this.$delete(this.productDetailsList, productDetailIndex);
                EventBus.$emit('update-mini-cart', "delete"); 
            },

            clearCart() {
                this.products = [];
                this.productDetailsList = [];
                EventBus.$emit('update-mini-cart', "clear"); 
            },

            submitForm() {
                localStorage.clear();               
            },

            getShoppingCartProducts(culture) {
                this.loading = true;
                axios.get("/api/ApiProduct/ShoppingCart?culture=" + culture + "&productSkus=" + this.products.toString())
                    .then(response => { this.productDetailsList = response.data; })
                    .catch(error => { this.errorMessage = error.response.data; })
                    .finally(() => { this.loading = false; });
            },

            getMainCategories(culture) {
                this.loadingMainCatgs = true;
                axios.get("/api/ApiProduct/MainCategories?culture=" + culture + "&includeSubCategories=false")
                    .then(response => { this.mainCategories = response.data; })
                    .catch(error => { this.errorMessage = error.response.data; })
                    .finally(() => { this.loadingMainCatgs = false; this.getImageAdvert(culture, "ProductPageLeftSide"); });
            },

            getImageAdvert(culture, advertType) {
                axios.get("/api/ApiGeneral/ImageAdvert?culture=" + culture + '&advertType=' + advertType)
                    .then(response => { if (response.status === 200) { this.advertLeft = response.data; } });
            },

            markdownToHtml(markdown) {
                let converter = new showdown.Converter();
                let html = converter.makeHtml(markdown);
                return html;
            }
           
        }
    });

}
