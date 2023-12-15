using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyGrid : MonoBehaviour {

    public AssemblyPanel assemblyPrefab;
    private List<AssemblyPanel> assemblies;

    void Start() {
        assemblies = new List<AssemblyPanel>();
    }

    public void PopulateAssemblies(List<RecipeCount> recipes) {

        for (int i = transform.childCount - 1; i >= 0; i--) {
            Destroy(transform.GetChild(i).gameObject);
        }

        foreach (RecipeCount recipeCount in recipes) {
            AddAssembly(recipeCount.recipe);
        }
    }

    public void AddAssembly(Recipe recipe) {

        // Don't create a panel with a duplicate recipe
        for (int i = 0; i < assemblies.Count; i++) {
            if (assemblies[i].recipe == recipe) return;
        }

        AssemblyPanel assembly = Instantiate(assemblyPrefab, transform);
        assembly.Initialize(recipe);

        assemblies.Add(assembly);
    }
}
