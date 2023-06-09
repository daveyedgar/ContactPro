﻿using ContactPro.Data;
using ContactPro.Models;
using ContactPro.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactPro.Services
{
    public class AddressBookService : IAddressBookService
    {
        private readonly ApplicationDbContext _context;

        public AddressBookService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddContactToCategoryAsync(int categoryId, int contactId)
        {
            // does this contact have this category already? use IsContactInCategory 
            try
            {
                // if it is null, or the contact is not in this category it adds it
                // otherwise it means it is already there and skips it
                if (!await IsContactInCategory(contactId, categoryId))
                {
                    Contact? contact = await _context.Contacts.FindAsync(contactId);
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if (contact != null && category != null)
                    {
                        category.Contacts.Add(contact);
                        await _context.SaveChangesAsync();
                    }

                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ICollection<Category>> GetContactCategoriesAsync(int contactId)
        {
            try
            {
                Contact? contact = await _context.Contacts.Include(c => c.Categories)
                                                           .FirstOrDefaultAsync(c => c.Id == contactId);
                return contact.Categories;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ICollection<int>> GetContactCategoryIdsAsync(int contactId)
        {
            try
            {
                var contact = await _context.Contacts
                                            .Include(c => c.Categories)
                                            .FirstOrDefaultAsync(c => c.Id == contactId);

                List<int> categoryIds = contact!.Categories.Select(c => c.Id).ToList();
                return categoryIds;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Category>> GetUserCategoriesAsync(string userId)
        {
            //new blank category list
            List<Category> categories = new List<Category>();

            try
            {
                //do to db => filter with where clause => this user = this id
                categories = await _context.Categories.Where(c => c.AppUserId == userId)
                                                      .OrderBy(c => c.Name)
                                                      .ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }

            return categories;
        }

        public async Task<bool> IsContactInCategory(int categoryId, int contactId)
        {
            Contact? contact = await _context.Contacts.FindAsync(contactId);

            return await _context.Categories
                                 .Include(c => c.Contacts)
                                 .Where(c => c.Id == categoryId && c.Contacts.Contains(contact))
                                 .AnyAsync();
        }

        public async Task RemoveContactFromCategoryAsync(int categoryId, int contactId)
        {
            try
            {
                if (await IsContactInCategory(categoryId, contactId))
                {
                    Contact contact = await _context.Contacts.FindAsync(contactId);

                    Category category = await _context.Categories.FindAsync(categoryId);

                    if (contact != null && category != null)
                    {
                        category.Contacts.Remove(contact);
                        await _context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public IEnumerable<Contact> SearchForContacts(string searchString, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
