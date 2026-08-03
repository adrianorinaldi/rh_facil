<template>
  <div class="employees-page">
    <div class="flex justify-content-between align-items-center mb-4">
      <h2 class="text-2xl font-bold m-0">Lista de Colaboradores</h2>
      <Button label="Novo Colaborador" icon="pi pi-plus" @click="openModal()" />
    </div>

    <!-- Tabela Profissional PrimeVue -->
    <Card>
      <template #content>
        <DataTable :value="employees" :loading="loading" paginator :rows="10" 
                   dataKey="id" :rowsPerPageOptions="[5, 10, 20]"
                   emptyMessage="Nenhum colaborador encontrado.">
          
          <Column field="id" header="ID" sortable></Column>
          <Column field="name" header="Nome" sortable>
            <template #body="{ data }">
              <span class="font-bold">{{ data.name }}</span>
            </template>
          </Column>
          <Column field="position" header="Cargo" sortable></Column>
          <Column field="department" header="Departamento" sortable></Column>
          <Column field="salary" header="Salário" sortable>
            <template #body="{ data }">
              R$ {{ data.salary.toFixed(2) }}
            </template>
          </Column>
          <Column field="status" header="Status" sortable>
            <template #body="{ data }">
              <Tag :severity="getSeverity(data.status)" :value="data.status" />
            </template>
          </Column>
          <Column header="Ações" :exportable="false" style="min-width:8rem">
            <template #body="{ data }">
              <Button icon="pi pi-pencil" outlined rounded class="mr-2" @click="openModal(data)" />
              <Button icon="pi pi-trash" outlined rounded severity="danger" @click="deleteEmployee(data.id)" />
            </template>
          </Column>

        </DataTable>
      </template>
    </Card>

    <!-- Modal de Formulário -->
    <Dialog v-model:visible="showModal" :header="isEditing ? 'Editar Colaborador' : 'Novo Colaborador'" 
            :modal="true" :style="{ width: '450px' }">
      
      <div class="flex flex-column gap-3 py-3">
        <div class="flex flex-column gap-2">
          <label for="name">Nome Completo</label>
          <InputText id="name" v-model="form.name" required />
        </div>
        <div class="flex flex-column gap-2">
          <label for="position">Cargo</label>
          <InputText id="position" v-model="form.position" required />
        </div>
        <div class="flex flex-column gap-2">
          <label for="department">Departamento</label>
          <InputText id="department" v-model="form.department" required />
        </div>
        <div class="flex flex-column gap-2">
          <label for="salary">Salário (R$)</label>
          <InputNumber id="salary" v-model="form.salary" mode="currency" currency="BRL" locale="pt-BR" />
        </div>
        <div class="flex flex-column gap-2">
          <label for="status">Status</label>
          <Select id="status" v-model="form.status" :options="['Ativo', 'Férias', 'Desligado']" />
        </div>
      </div>

      <template #footer>
        <Button label="Cancelar" icon="pi pi-times" text @click="closeModal" />
        <Button label="Salvar" icon="pi pi-check" @click="saveEmployee" />
      </template>
    </Dialog>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import Button from 'primevue/button'
import Card from 'primevue/card'
import DataTable from 'primevue/datatable'
import Column from 'primevue/column'
import Tag from 'primevue/tag'
import Dialog from 'primevue/dialog'
import InputText from 'primevue/inputtext'
import InputNumber from 'primevue/inputnumber'
import Select from 'primevue/select'

const API_URL = 'http://localhost:5031/api/employees'
const employees = ref([])
const loading = ref(false)
const showModal = ref(false)
const isEditing = ref(false)

const form = ref({
  id: 0,
  name: '',
  position: '',
  department: '',
  salary: 0,
  status: 'Ativo'
})

const getSeverity = (status) => {
  if (status === 'Ativo') return 'success'
  if (status === 'Férias') return 'warn'
  return 'danger'
}

const fetchEmployees = async () => {
  loading.value = true
  try {
    const res = await fetch(API_URL)
    employees.value = await res.json()
  } catch (error) {
    console.error('Erro ao buscar:', error)
  } finally {
    loading.value = false
  }
}

const openModal = (emp = null) => {
  if (emp) {
    isEditing.value = true
    form.value = { ...emp }
  } else {
    isEditing.value = false
    form.value = { id: 0, name: '', position: '', department: '', salary: 0, status: 'Ativo' }
  }
  showModal.value = true
}

const closeModal = () => {
  showModal.value = false
}

const saveEmployee = async () => {
  try {
    const method = isEditing.value ? 'PUT' : 'POST'
    const url = isEditing.value ? `${API_URL}/${form.value.id}` : API_URL
    
    await fetch(url, {
      method,
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(form.value)
    })
    
    closeModal()
    fetchEmployees()
  } catch (error) {
    console.error('Erro ao salvar:', error)
  }
}

const deleteEmployee = async (id) => {
  if (confirm('Tem certeza que deseja excluir?')) {
    try {
      await fetch(`${API_URL}/${id}`, { method: 'DELETE' })
      fetchEmployees()
    } catch (error) {
      console.error('Erro ao excluir:', error)
    }
  }
}

onMounted(() => {
  fetchEmployees()
})
</script>

<style scoped>
/* Utilitários Flex (simulando PrimeFlex que não instalamos) */
.flex { display: flex; }
.flex-column { flex-direction: column; }
.justify-content-between { justify-content: space-between; }
.align-items-center { align-items: center; }
.gap-2 { gap: 0.5rem; }
.gap-3 { gap: 1rem; }
.mb-4 { margin-bottom: 2rem; }
.mr-2 { margin-right: 0.5rem; }
.py-3 { padding-top: 1rem; padding-bottom: 1rem; }
.m-0 { margin: 0; }
.text-2xl { font-size: 1.5rem; }
.font-bold { font-weight: bold; }
</style>
