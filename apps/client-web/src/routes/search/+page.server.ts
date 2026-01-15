export const load = async ({ url }) => {
    const q = url.searchParams.get('q') || '';

    // TODO: Call Catalog Service API here
    // const res = await fetch(`http://localhost:8080/catalog/search?q=${q}`);

    return {
        results: [
            { id: 1, name: "Sunset Photos", category: "Photography", rating: 4.8, description: "Capturing moments that last a lifetime.", basePrice: 1500 },
            { id: 2, name: "Delicious Bites", category: "Catering", rating: 4.5, description: "Gourmet catering for all occasions.", basePrice: 2500 },
            { id: 3, name: "DJ Mike", category: "Music", rating: 4.9, description: "Keep the party going all night long!", basePrice: 800 },
            { id: 4, name: "Floral Dreams", category: "Florist", rating: 4.7, description: "Beautiful arrangements for your special day.", basePrice: 600 },
        ]
    };
};
