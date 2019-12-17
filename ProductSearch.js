function ActivateProductSearch(culture, resourceStrings) {

    Vue.use(VueLineClamp);
    Vue.use(VueClazyLoad);

    var delayTimer;

    var appSearch = new Vue({
        el: '#productsearch',
        mounted: function () {
            this.culture = culture;
            this.resourceStrings = JSON.parse(resourceStrings);

            // Load Main Category Menu Data
            this.getMainCategories(culture);

        },
        components: { VueIntlNumberformat: VueIntlNumberformat },
        data: function () {
            return {
                culture: "",
                searchText: "",
                productsExist: false,
                loadingProducts: false,
                loadingMainCatgs: false,
                advertLeft: {},
                mainCategories: [],
                products: []
            };
        },
        methods: {

            textChanged(inputText) {

                // Clear Products if no search criteria
                if (inputText.length === 0) { this.products = []; }

                // If text changed check if inout is 3 or more chars and wait one second before calling API             
                var vjsThis = this;
                if (delayTimer) { clearTimeout(delayTimer); delayTimer = null; }
                if (inputText.length >= 3) {
                    delayTimer = setTimeout(function () { vjsThis.getProductSearch(inputText); }, 500);
                }
            },

            getProductSearch(searchText) {
                this.loadingProducts = true;
                axios.get("/api/ApiProduct/ProductSearch?culture=" + this.culture + "&searchText=" + searchText)
                    .then(response => {
                        if (response.status !== 200) { this.productsExist = false; }
                        else { this.products = response.data; this.productsExist = true; }
                    })
                    .catch(() => { this.productsExist = false; })
                    .finally(() => { this.loadingProducts = false; });
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
            }

        }
    });

}