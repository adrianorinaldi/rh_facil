<script setup>
import { ref, onMounted, computed } from 'vue';

const employees = ref([]);
const loading = ref(true);

const fetchEmployees = async () => {
    try {
        const response = await fetch('/api/employees');
        const data = await response.json();
        employees.value = data;
    } catch (error) {
        console.error('Erro ao buscar funcionários:', error);
    } finally {
        loading.value = false;
    }
};

const totalEmployees = computed(() => employees.value.length);
const activeEmployees = computed(() => employees.value.filter(e => e.status === 'Ativo').length);

const recentHires = computed(() => {
    return [...employees.value]
        .sort((a, b) => new Date(b.hireDate) - new Date(a.hireDate))
        .slice(0, 5); // Últimas 5 contratações
});

onMounted(() => {
    fetchEmployees();
});
</script>

<template>
    <div class="grid grid-cols-12 gap-8">
        <!-- Boas-vindas -->
        <div class="col-span-12">
            <div class="card bg-primary text-primary-contrast rounded-border p-6 text-center">
                <h1 class="text-3xl font-bold mb-2">Bem-vindo ao RH Fácil! 👋</h1>
                <p class="text-lg opacity-80">Seu painel simplificado para gestão de colaboradores.</p>
            </div>
        </div>

        <!-- Estatísticas -->
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Total de Colaboradores</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl">
                            <i class="pi pi-spin pi-spinner" v-if="loading"></i>
                            <span v-else>{{ totalEmployees }}</span>
                        </div>
                    </div>
                    <div class="flex items-center justify-center bg-blue-100 dark:bg-blue-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-users text-blue-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>
        
        <div class="col-span-12 lg:col-span-6 xl:col-span-3">
            <div class="card mb-0">
                <div class="flex justify-between mb-4">
                    <div>
                        <span class="block text-muted-color font-medium mb-4">Colaboradores Ativos</span>
                        <div class="text-surface-900 dark:text-surface-0 font-medium text-xl">
                            <i class="pi pi-spin pi-spinner" v-if="loading"></i>
                            <span v-else>{{ activeEmployees }}</span>
                        </div>
                    </div>
                    <div class="flex items-center justify-center bg-green-100 dark:bg-green-400/10 rounded-border" style="width: 2.5rem; height: 2.5rem">
                        <i class="pi pi-check-circle text-green-500 !text-xl"></i>
                    </div>
                </div>
            </div>
        </div>

        <!-- Painel de Contratações Recentes -->
        <div class="col-span-12 xl:col-span-6">
            <div class="card">
                <div class="font-semibold text-xl mb-4">🎉 Contratações Recentes</div>
                <div v-if="loading" class="flex justify-center p-4">
                    <i class="pi pi-spin pi-spinner text-3xl"></i>
                </div>
                <div v-else-if="recentHires.length === 0" class="text-center p-4 text-muted-color">
                    Nenhum colaborador encontrado.
                </div>
                <DataTable v-else :value="recentHires" :rows="5" responsiveLayout="scroll">
                    <Column field="name" header="Nome"></Column>
                    <Column field="position" header="Cargo"></Column>
                    <Column header="Data">
                        <template #body="slotProps">
                            {{ new Date(slotProps.data.hireDate).toLocaleDateString('pt-BR') }}
                        </template>
                    </Column>
                </DataTable>
            </div>
        </div>
        
        <!-- Painel Dicas -->
        <div class="col-span-12 xl:col-span-6">
            <div class="card">
                <div class="font-semibold text-xl mb-4">💡 Dica do Dia</div>
                <p class="text-surface-600 dark:text-surface-200 leading-normal">
                    Mantenha os dados dos seus colaboradores sempre atualizados! Utilize a tela de <router-link to="/colaboradores" class="text-primary hover:underline">Colaboradores</router-link> para registrar promoções, mudanças de departamento e ajustes de salário.
                </p>
                <div class="mt-4 flex justify-center text-primary">
                    <i class="pi pi-lightbulb" style="font-size: 5rem; opacity: 0.2"></i>
                </div>
            </div>
        </div>
    </div>
</template>
