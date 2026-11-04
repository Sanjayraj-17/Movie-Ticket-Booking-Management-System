import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TheatreService } from '../../../services/theatre.service';
import { AuthService } from '../../../services/auth.service';
import { Theatre, CreateTheatreDto, UpdateTheatreDto } from '../../../models/theatre';

@Component({
  selector: 'app-admin-theatres',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './admin-theatres.component.html',
  styleUrl: './admin-theatres.component.scss'
})
export class AdminTheatresComponent implements OnInit {
  // All theatres from backend
  theatres: Theatre[] = [];
  filteredTheatres: Theatre[] = [];

  // Filter & Search states
  searchQuery: string = '';
  selectedCity: string = 'All';
  availableCities: string[] = ['All'];

  // Loading & Alert states
  isLoading: boolean = false;
  isSaving: boolean = false;
  isDeleting: boolean = false;
  errorMessage: string = '';
  successMessage: string = '';

  // Modal / Form state
  showModal: boolean = false;
  isEditMode: boolean = false;
  currentTheatreId: number | null = null;

  // Form Fields
  theatreForm = {
    name: '',
    city: '',
    address: ''
  };

  // Delete Confirmation state
  theatreToDelete: Theatre | null = null;

  constructor(
    public authService: AuthService,
    private theatreService: TheatreService
  ) {}

  ngOnInit(): void {
    this.loadTheatres();
  }

  // Load all theatres from backend API
  loadTheatres(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.theatreService.getAllTheatres().subscribe({
      next: (data) => {
        this.theatres = data;
        this.extractCities();
        this.applyFilter();
        this.isLoading = false;
      },
      error: (err) => {
        console.error('Error loading theatres:', err);
        this.errorMessage = 'Failed to load cinema theatres from backend.';
        this.isLoading = false;
      }
    });
  }

  // Extract unique cities for the dropdown filter
  extractCities(): void {
    const citiesSet = new Set<string>();
    this.theatres.forEach((t) => {
      if (t.city && t.city.trim()) {
        citiesSet.add(t.city.trim());
      }
    });
    this.availableCities = ['All', ...Array.from(citiesSet).sort()];
  }

  // Filter theatres by keyword and selected city
  applyFilter(): void {
    let list = [...this.theatres];

    if (this.searchQuery.trim()) {
      const q = this.searchQuery.toLowerCase().trim();
      list = list.filter((t) =>
        t.name.toLowerCase().includes(q) ||
        t.city.toLowerCase().includes(q) ||
        t.address.toLowerCase().includes(q)
      );
    }

    if (this.selectedCity !== 'All') {
      list = list.filter((t) => t.city.toLowerCase() === this.selectedCity.toLowerCase());
    }

    this.filteredTheatres = list;
  }

  // Open modal for Adding a new theatre
  openAddModal(): void {
    this.isEditMode = false;
    this.currentTheatreId = null;
    this.theatreForm = {
      name: '',
      city: '',
      address: ''
    };
    this.showModal = true;
  }

  // Open modal for Editing an existing theatre
  openEditModal(theatre: Theatre): void {
    this.isEditMode = true;
    this.currentTheatreId = theatre.id;
    this.theatreForm = {
      name: theatre.name,
      city: theatre.city,
      address: theatre.address
    };
    this.showModal = true;
  }

  // Close form modal
  closeModal(): void {
    this.showModal = false;
  }

  // Submit Add or Edit Form
  saveTheatre(): void {
    if (!this.theatreForm.name.trim()) {
      this.errorMessage = 'Theatre name is required.';
      return;
    }

    if (!this.theatreForm.city.trim()) {
      this.errorMessage = 'City is required.';
      return;
    }

    if (!this.theatreForm.address.trim()) {
      this.errorMessage = 'Address is required.';
      return;
    }

    this.isSaving = true;
    this.errorMessage = '';

    if (this.isEditMode && this.currentTheatreId) {
      // Update existing theatre via PUT /api/theatres/{id}
      const updateDto: UpdateTheatreDto = {
        name: this.theatreForm.name.trim(),
        city: this.theatreForm.city.trim(),
        address: this.theatreForm.address.trim()
      };

      this.theatreService.updateTheatre(this.currentTheatreId, updateDto).subscribe({
        next: () => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = `Theatre "${this.theatreForm.name}" updated successfully.`;
          this.loadTheatres();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to update theatre.';
        }
      });
    } else {
      // Create new theatre via POST /api/theatres
      const createDto: CreateTheatreDto = {
        name: this.theatreForm.name.trim(),
        city: this.theatreForm.city.trim(),
        address: this.theatreForm.address.trim()
      };

      this.theatreService.createTheatre(createDto).subscribe({
        next: (created) => {
          this.isSaving = false;
          this.showModal = false;
          this.successMessage = `Theatre "${created.name}" created successfully.`;
          this.loadTheatres();
          setTimeout(() => (this.successMessage = ''), 4000);
        },
        error: (err) => {
          this.isSaving = false;
          this.errorMessage = err.error?.message || 'Failed to create theatre.';
        }
      });
    }
  }

  // Open delete confirmation modal
  confirmDelete(theatre: Theatre): void {
    this.theatreToDelete = theatre;
  }

  // Cancel deletion
  cancelDelete(): void {
    this.theatreToDelete = null;
  }

  // Execute deletion via DELETE /api/theatres/{id}
  executeDelete(): void {
    if (!this.theatreToDelete) return;

    this.isDeleting = true;
    this.errorMessage = '';

    this.theatreService.deleteTheatre(this.theatreToDelete.id).subscribe({
      next: () => {
        this.isDeleting = false;
        this.successMessage = `Theatre "${this.theatreToDelete?.name}" deleted successfully.`;
        this.theatreToDelete = null;
        this.loadTheatres();
        setTimeout(() => (this.successMessage = ''), 4000);
      },
      error: (err) => {
        this.isDeleting = false;
        this.errorMessage = err.error?.message || 'Failed to delete theatre. Ensure no active screens or shows are attached.';
      }
    });
  }
}
