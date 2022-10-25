/*
 * Pager for ASP.NET MVC
 * source : http://www.superstarcoders.com/blogs/posts/pager-for-asp-net-mvc.aspx
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace GitCandy.Base;

/// <summary>分页类</summary>
public sealed class Pager : IEnumerable<Int32>
{
    private Int32 _numberOfPages;
    private Int32 _skipPages;
    private Int32 _takePages;
    private Int32 _currentPageIndex;
    private Int32 _numberOfItems;
    private Int32 _itemsPerPage;

    private Pager()
    {
    }

    private Pager(Pager pager)
    {
        _numberOfItems = pager._numberOfItems;
        _currentPageIndex = pager._currentPageIndex;
        _numberOfPages = pager._numberOfPages;
        _takePages = pager._takePages;
        _skipPages = pager._skipPages;
        _itemsPerPage = pager._itemsPerPage;
    }

    /// <summary>
    /// Creates a pager for the given number of items.
    /// </summary>
    public static Pager Items(Int32 numberOfItems)
    {
        return new Pager
        {
            _numberOfItems = numberOfItems,
            _currentPageIndex = 1,
            _numberOfPages = 1,
            _skipPages = 0,
            _takePages = 1,
            _itemsPerPage = numberOfItems
        };
    }

    /// <summary>
    /// Specifies the number of items per page.
    /// </summary>
    public Pager PerPage(Int32 itemsPerPage)
    {
        var numberOfPages = (_numberOfItems + itemsPerPage - 1) / itemsPerPage;

        return new Pager(this)
        {
            _numberOfPages = numberOfPages,
            _skipPages = 0,
            _takePages = numberOfPages - _currentPageIndex + 1,
            _itemsPerPage = itemsPerPage
        };
    }

    /// <summary>
    /// Moves the pager to the given page index
    /// </summary>
    public Pager Move(Int32 pageIndex)
    {
        return new Pager(this)
        {
            _currentPageIndex = pageIndex
        };
    }

    /// <summary>
    /// Segments the pager so that it will display a maximum number of pages.
    /// </summary>
    public Pager Segment(Int32 maximum)
    {
        var count = Math.Min(_numberOfPages, maximum);

        return new Pager(this)
        {
            _takePages = count,
            _skipPages = Math.Min(_skipPages, _numberOfPages - count),
        };
    }

    /// <summary>
    /// Centers the segment around the current page
    /// </summary>
    /// <returns></returns>
    public Pager Center()
    {
        var radius = ((_takePages + 1) / 2);

        return new Pager(this)
        {
            _skipPages = Math.Min(Math.Max(_currentPageIndex - radius, 0), _numberOfPages - _takePages)
        };
    }

    public IEnumerator<Int32> GetEnumerator() => Enumerable.Range(_skipPages + 1, _takePages).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public Boolean IsPaged => _numberOfItems > _itemsPerPage;

    public Int32 NumberOfPages => _numberOfPages;

    public Boolean IsUnpaged => _numberOfPages == 1;

    public Int32 CurrentPageIndex => _currentPageIndex;

    public Int32 NextPageIndex => _currentPageIndex + 1;

    public Int32 LastPageIndex => _numberOfPages;

    public Int32 FirstPageIndex => 1;

    public Boolean HasNextPage => _currentPageIndex < _numberOfPages && _numberOfPages > 1;

    public Boolean HasPreviousPage => _currentPageIndex > 1 && _numberOfPages > 1;

    public Int32 PreviousPageIndex => _currentPageIndex - 1;

    public Boolean IsSegmented => _skipPages > 0 || _skipPages + 1 + _takePages < _numberOfPages;

    public Boolean IsEmpty => _numberOfPages < 1;

    public Boolean IsFirstSegment => _skipPages == 0;

    public Boolean IsLastSegment => _skipPages + 1 + _takePages >= _numberOfPages;
}